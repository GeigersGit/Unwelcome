/* Written by Kaz Crowe */
/* SimpleJoystick.cs */
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/* 
 * First off, we are using [ExecuteInEditMode] to be able to show changes in real time.
 * This will not affect anything within a build or play mode. This simply makes the script
 * able to be run while in the Editor in Edit Mode.
*/
[ExecuteInEditMode]
public class SimpleJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	/* ----- > ASSIGNED VARIABLES < ----- */
	public RectTransform joystick, joystickSizeFolder, joystickBase;
	RectTransform baseTrans;
	Vector3 joystickCenter;
	Vector2 textureCenter, defaultPos;
	
	/* ----- > SIZE AND PLACEMENT < ----- */
	public enum ScalingAxis{ Width, Height }
	public ScalingAxis scalingAxis = ScalingAxis.Height;
	public enum Anchor{ Left, Right }
	public Anchor anchor;
	public enum JoystickTouchSize{ Default, Medium, Large, Custom }
	public JoystickTouchSize joystickTouchSize;
	public float joystickSize = 2.0f, radiusModifier = 4.5f;
	float radius = 0.0f;
	public bool dynamicPositioning = false;
	public float customTouchSize_X = 50.0f, customTouchSize_Y = 75.0f;
	public float customTouchSizePos_X = 0.0f, customTouchSizePos_Y = 0.0f;
	public float customSpacing_X = 5.0f, customSpacing_Y = 20.0f;

	/* ----- > STYLE AND OPTIONS < ----- */
	public bool touchPad = false;
	public enum Axis{ Both, X, Y }
	public Axis axis = Axis.Both;
	public enum Boundary{ Circular, Square }
	public Boundary boundary = Boundary.Circular;
	public enum DeadZoneOption{ DoNotUse, OneValue, TwoValues }
	public DeadZoneOption deadZoneOption = DeadZoneOption.DoNotUse;
	public float xDeadZone = 0.1f, yDeadZone = 0.1f;

	/* ----- > SCRIPT REFERENCE < ----- */
	static Dictionary<string,SimpleJoystick> SimpleJoysticks = new Dictionary<string, SimpleJoystick>();
	public string joystickName;
	Vector2 currentJoystickPosition = new Vector2();
	
	public bool exposeValues = false;
	public float horizontalValue = 0.0f;
	public float verticalValue = 0.0f;
	public enum JoystickPreset{ Default, TouchPad, Classic }
	public JoystickPreset joystickPreset;
	public bool joystickState = false;
	int _pointerId = -10;// Default value of -10

	
	void Awake ()
	{
		// If the Application is playing, then register this joystick.
		if( Application.isPlaying == true && joystickName != string.Empty )
			RegisterJoystick( joystickName );
	}

	void Start ()
	{
		if( Application.isPlaying == false )
			return;

		// UpdatePositioning of the joystick on Start().
		UpdatePositioning();

		if( !GetParentCanvas().GetComponent<SimpleJoystickUpdater>() )
			GetParentCanvas().gameObject.AddComponent( typeof( SimpleJoystickUpdater ) );
	}
	
	// This function is called on the initial touch down.
	public void OnPointerDown ( PointerEventData touchInfo )
	{
		// If the joystick is already in use, then return.
		if( joystickState == true )
			return;

		// Set the joystick state since the joystick is being interacted with.
		joystickState = true;

		// Assign the pointerId so that the other functions can know if the pointer calling the function is the correct one.
		_pointerId = touchInfo.pointerId;

		// If dynamicPositioning or touch pad are enabled...
		if( dynamicPositioning == true || touchPad == true )
		{
			// Then move the joystickSizeFolder to the position of the touch.
			joystickSizeFolder.position = touchInfo.position - textureCenter;

			// Set the joystickCenter so that the position can be calculated correctly.
			joystickCenter = touchInfo.position;
		}

		// Call UpdateJoystick with the info from the current PointerEventData.
		UpdateJoystick( touchInfo );
	}

	// This function is called when the user is dragging the joystick.
	public void OnDrag ( PointerEventData touchInfo )
	{
		// If the pointer event that is calling this function is not the same as the one that initiated the joystick, then return.
		if( touchInfo.pointerId != _pointerId )
			return;

		// Call UpdateJoystick with the info from the current PointerEventData.
		UpdateJoystick( touchInfo );
	}

	// This function is called when the user has released the touch.
	public void OnPointerUp ( PointerEventData touchInfo )
	{
		// If the pointer event that is calling this function is not the same as the one that initiated the joystick, then return.
		if( touchInfo.pointerId != _pointerId )
			return;

		// Since the touch has lifted, set the state to false and reset the local pointerId.
		joystickState = false;
		_pointerId = -10;

		// If dynamicPositioning, touch pad, or draggable are enabled...
		if( dynamicPositioning == true || touchPad == true )
		{
			// The joystickSizeFolder needs to be reset back to the default position.
			joystickSizeFolder.position = defaultPos;

			// Reset the joystickCenter since the touch has been released.
			joystickCenter = joystickBase.position;
		}

		// Reset the joystick's position back to center.
		joystick.position = joystickCenter;

		// Set the reference to the x and y position so they can be referenced from PlayMaker or whatever else..
		if( exposeValues == true )
		{
			horizontalValue = 0.0f;
			verticalValue = 0.0f;
		}

		UpdateJoystickPosition();
	}

	// This function updates the joystick according to the current touch.
	void UpdateJoystick ( PointerEventData touchInfo )
	{
		// Create a new Vector2 to equal the vector from the current touch to the center of joystick.
		Vector2 tempVector = touchInfo.position - ( Vector2 )joystickCenter;

		// If the user wants only one axis, then zero out the opposite value.
		if( axis == Axis.X )
			tempVector.y = 0;
		else if( axis == Axis.Y )
			tempVector.x = 0;

		// If the user wants a circular boundary for the joystick, then clamp the magnitude by the radius.
		if( boundary == Boundary.Circular )
			tempVector = Vector2.ClampMagnitude( tempVector, radius );
		// Else the user wants a square boundary, so clamp X and Y individually.
		else if( boundary == Boundary.Square )
		{
			tempVector.x = Mathf.Clamp( tempVector.x,  -radius,  radius );
			tempVector.y = Mathf.Clamp( tempVector.y,  -radius,  radius );
		}

		// Apply the tempVector to the joystick's position.
		joystick.transform.position = ( Vector2 )joystickCenter + tempVector;

		if( exposeValues == true )
		{
			horizontalValue = GetPosition().x;
			verticalValue = GetPosition().y;
		}

		UpdateJoystickPosition();
	}

	// This function will configure the position of an image based on the size and custom spacing selected.
	Vector2 ConfigureImagePosition ( Vector2 textureSize, Vector2 customSpacing )
	{
		// First, fix the customSpacing to be a value between 0.0f and 1.0f.
		Vector2 fixedCustomSpacing = customSpacing / 100;

		// Then configure position spacers according to screen size, the fixed spacing and texture size.
		float positionSpacerX = Screen.width * fixedCustomSpacing.x - ( textureSize.x * fixedCustomSpacing.x );
		float positionSpacerY = Screen.height * fixedCustomSpacing.y - ( textureSize.y * fixedCustomSpacing.y );

		// Create a temporary Vector2 to modify and return.
		Vector2 tempVector;

		// If it's left, simply apply the positionxSpacerX, else calculate out from the right side and apply the positionSpaceX.
		tempVector.x = anchor == Anchor.Left ? positionSpacerX : ( Screen.width - textureSize.x ) - positionSpacerX;

		// Apply the positionSpacerY variable.
		tempVector.y = positionSpacerY;

		// Return the updated temporary Vector.
		return tempVector;
	}

	// This function is used only to find the canvas parent if its not the root object.
	Canvas GetParentCanvas ()
	{
		Transform parent = transform.parent;
		while( parent != null )
		{ 
			if( parent.transform.GetComponent<Canvas>() )
				return parent.transform.GetComponent<Canvas>();

			parent = parent.transform.parent;
		}
		return null;
	}
	
	Vector2 JoystickPositionDeadZone
	{
		get
		{
			// Store the joystick's position in a temporary Vector2.
			Vector2 tempVec = ( joystick.position - joystickCenter ) / radius;
			
			// If the X value is to the LEFT, then update the deadZone vector2.x to -1 if it is.
			if( tempVec.x < -xDeadZone )
				tempVec.x = -1;
			// Else check if it is to the RIGHT, then update the deadZone vector2.x to 1 if it is.
			else if( tempVec.x > xDeadZone )
				tempVec.x = 1;
			// Else it is not past the deadZone values, so set it to zero.
			else
				tempVec.x = 0;
			
			// If the Y value is DOWN and then update the deadZone vector2.y to -1 if it is.
			if( tempVec.y < -yDeadZone )
				tempVec.y = -1;
			// Else check if it is UP, then update the deadZone vector2.y to 1 if it is.
			else if( tempVec.y > yDeadZone )
				tempVec.y = 1;
			// Else it is not past the deadZone values, so set it to zero.
			else
				tempVec.y = 0;
			
			return tempVec;
		}
	}

	void RegisterJoystick ( string joystickName )
	{
		if( SimpleJoysticks.ContainsKey( joystickName ) )
			SimpleJoysticks.Remove( joystickName );

		SimpleJoysticks.Add( joystickName, GetComponent<SimpleJoystick>() );
	}

	void UpdateJoystickPosition ()
	{
		if( deadZoneOption != DeadZoneOption.DoNotUse )
			currentJoystickPosition = JoystickPositionDeadZone;
		else
			currentJoystickPosition = ( joystick.position - joystickCenter ) / radius;
	}

#if UNITY_EDITOR
	void Update ()
	{
		// Keep the joystick updated while the game is not being played.
		if( Application.isPlaying == false )
			UpdatePositioning();
	}
#endif

	/* --------------------------------------------- *** PUBLIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Updates the size and placement of the Simple Joystick. Useful for screen rotations or changing of joystick options.
	/// </summary>
	public void UpdatePositioning ()
	{
		if( joystickSizeFolder == null || joystickBase == null || joystick == null )
			return;
		
		float referenceSize = scalingAxis == ScalingAxis.Height ? Screen.height : Screen.width;
		
		// Configure a size for the image based on the Canvas's size and scale.
		float textureSize = referenceSize * ( joystickSize / 10 );
		
		// If baseTrans is null, store this object's RectTrans so that it can be positioned.
		if( baseTrans == null )
			baseTrans = GetComponent<RectTransform>();
		
		// Get a position for the joystick based on the position variables.
		Vector2 imagePosition = ConfigureImagePosition( new Vector2( textureSize, textureSize ), new Vector2( customSpacing_X, customSpacing_Y ) );
		
		// If the user wants a custom touch size...
		if( joystickTouchSize == JoystickTouchSize.Custom )
		{
			// Fix the custom size variables.
			float fixedFBPX = customTouchSize_X / 100;
			float fixedFBPY = customTouchSize_Y / 100;
			
			// Depending on the joystickTouchSize options, configure the size.
			baseTrans.sizeDelta = new Vector2( Screen.width * fixedFBPX, Screen.height * fixedFBPY );
			
			// Send the size and custom positioning to the ConfigureImagePosition function to get the exact position.
			Vector2 imagePos = ConfigureImagePosition( baseTrans.sizeDelta, new Vector2( customTouchSizePos_X, customTouchSizePos_Y ) );

			// Apply the new position.
			baseTrans.position = imagePos;
		}
		else
		{
			// Temporary float to store a modifier for the touch area size.
			float fixedTouchSize = joystickTouchSize == JoystickTouchSize.Large ? 2.0f : joystickTouchSize == JoystickTouchSize.Medium ? 1.51f : 1.01f;
			
			// Temporary Vector2 to store the default size of the joystick.
			Vector2 tempVector = new Vector2( textureSize, textureSize );
			
			// Apply the joystick size multiplied by the fixedTouchSize.
			baseTrans.sizeDelta = tempVector * fixedTouchSize;
			
			// Apply the imagePosition modified with the difference of the sizeDelta divided by 2, multiplied by the scale of the parent canvas.
			baseTrans.position = imagePosition - ( ( baseTrans.sizeDelta - tempVector ) / 2 );
		}

		// If the options dictate that the default position needs to be stored...
		if( dynamicPositioning == true || touchPad == true )
		{
			// Set the texture center so that the joystick can move to the touch position correctly.
			textureCenter = new Vector2( textureSize / 2, textureSize / 2 );
			
			// Also need to store the default position so that it can return after the touch has been lifted.
			defaultPos = imagePosition;
		}

		// Store the pivot of the baseTrans so that the position will be correct no matter what the user has set for pivot.
		Vector2 pivotSpacer = new Vector2( baseTrans.sizeDelta.x * baseTrans.pivot.x, baseTrans.sizeDelta.y * baseTrans.pivot.y );
		
		// Apply the size and position to the joystickSizeFolder.
		joystickSizeFolder.sizeDelta = new Vector2( textureSize, textureSize );
		joystickSizeFolder.position = imagePosition + pivotSpacer;
		
		// Configure the size of the joystick's radius.
		radius = ( joystickSizeFolder.sizeDelta.x ) * ( radiusModifier / 10 );
		
		// Store the joystick's center so that JoystickPosition can be configured correctly.
		joystickCenter = joystickBase.position;

		joystick.position = joystickCenter;
	}

	/// <summary>
	/// Returns the position of the Simple Joystick in a Vector2 Variable. X = Horizontal, Y = Vertical.
	/// </summary>
	/// <value>The Simple Joystick's Position.</value>
	public Vector2 GetPosition ()
	{
		return currentJoystickPosition;
	}

	/// <summary>
	/// Returns the distance of the Simple Joystick from center.
	/// </summary>
	/// <value>The Simple Joystick's Distance.</value>
	public float GetDistance ()
	{
		return Vector3.Distance( joystick.position, joystickCenter ) / radius;
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the horizontal value of the Simple Joystick.
	/// </summary>
	public float GetHorizontalAxis ()
	{
		return GetPosition().x;
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the vertical value of the Simple Joystick.
	/// </summary>
	public float GetVerticalAxis ()
	{
		return GetPosition().y;
	}

	/// <summary>
	/// Returns the state of the Simple Joystick. This function will return true when the joystick is being interacted with, and false when not.
	/// </summary>
	public bool GetJoystickState ()
	{
		return joystickState;
	}

	/// <summary>
	/// Resets the Simple Joystick if it becomes stuck or needs to be released for some other reason.
	/// </summary>
	public void ResetJoystick ()
	{
		OnPointerUp( null );
	}

	/// <summary>
	/// Disables the Simple Joystick.
	/// </summary>
	public void DisableJoystick ()
	{
		// Set the states to false.
		joystickState = false;
		_pointerId = -10;
		
		// If the joystick center has been changed, then reset it.
		if( dynamicPositioning == true )
		{
			joystickSizeFolder.position = defaultPos;
			joystickCenter = joystickBase.position;
		}
		
		// Reset the position of the joystick.
		joystick.position = joystickCenter;
		
		// Reset the input values.
		horizontalValue = 0.0f;
		verticalValue = 0.0f;
		
		// Disable the gameObject.
		gameObject.SetActive( false );
	}

	/// <summary>
	/// Enables the Simple Joystick.
	/// </summary>
	public void EnableJoystick ()
	{
		// Reset the joystick's position again.
		joystick.position = joystickCenter;

		// Enable the gameObject.
		gameObject.SetActive( true );
	}
	/* ------------------------------------------- *** END PUBLIC FUNCTIONS FOR THE USER *** ------------------------------------------- */

	/* --------------------------------------------- *** STATIC FUNCTIONS FOR THE USER *** --------------------------------------------- */
	/// <summary>
	/// Returns the Simple Joystick with the targeted name if it exists within the scene.
	/// </summary>
	/// <param name="joystickName">The registered name of the desired Simple Joystick.</param>
	static public SimpleJoystick GetSimpleJoystick ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return null;

		return SimpleJoysticks[ joystickName ];
	}

	/// <summary>
	/// Gets the joystick position.
	/// </summary>
	/// <returns>The joystick position.</returns>
	/// <param name="joystickName">The Joystick Name of the desired Simple Joystick.</param>
	static public Vector2 GetPosition ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return Vector2.zero;
		
		return SimpleJoysticks[ joystickName ].GetPosition();
	}

	/// <summary>
	/// Returns the distance of the Simple Joystick from center.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Simple Joystick.</param>
	/// <returns>A value between 0 and 1 representing the distance of the joystick from it's center.</returns>
	static public float GetDistance ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;
		return SimpleJoysticks[ joystickName ].GetDistance();
	}

	/// <summary>
	/// Returns the state of the Simple Joystick.
	/// </summary>
	/// <param name="joystickName">The Joystick Name of the desired Simple Joystick.</param>
	/// <returns>Returns true when the joystick is being interacted with, and false when not.</returns>
	static public bool GetJoystickState ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return false;

		return SimpleJoysticks[ joystickName ].joystickState;
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the horizontal value of the Simple Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Simple Joystick.</param>
	public static float GetHorizontalAxis ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return SimpleJoysticks[ joystickName ].GetHorizontalAxis();
	}

	/// <summary>
	/// Returns a float value between -1 and 1 representing the vertical value of the Simple Joystick.
	/// </summary>
	/// <param name="joystickName">The name of the desired Simple Joystick.</param>
	public static float GetVerticalAxis ( string joystickName )
	{
		if( !JoystickConfirmed( joystickName ) )
			return 0.0f;

		return SimpleJoysticks[ joystickName ].GetVerticalAxis();
	}

	static bool JoystickConfirmed ( string joystickName )
	{
		if( !SimpleJoysticks.ContainsKey( joystickName ) )
		{
			Debug.LogWarning( "No Simple Joystick has been registered with the name: " + joystickName + "." );
			return false;
		}
		return true;
	}
	/* ------------------------------------------- *** END STATIC FUNCTIONS FOR THE USER *** ------------------------------------------- */
}

public class SimpleJoystickUpdater : UIBehaviour
{
	protected override void OnRectTransformDimensionsChange ()
	{
		StartCoroutine( "YieldPositioning" );
	}

	IEnumerator YieldPositioning ()
	{
		yield return new WaitForEndOfFrame();

		SimpleJoystick[] allJoysticks = FindObjectsOfType( typeof( SimpleJoystick ) ) as SimpleJoystick[];

		for( int i = 0; i < allJoysticks.Length; i++ )
			allJoysticks[ i ].UpdatePositioning();
	}
}