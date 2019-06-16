/* Written by Kaz Crowe */
/* SimpleJoystickEditor.cs */
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using UnityEngine.EventSystems;

[CanEditMultipleObjects]
[CustomEditor( typeof( SimpleJoystick ) )]
public class SimpleJoystickEditor : Editor
{
	SimpleJoystick sj;

	SerializedProperty joystickPreset;

	/* -----< ASSIGNED VARIABLES >----- */
	SerializedProperty joystick, joystickSizeFolder, joystickBase;
	
	/* -----< SIZE AND PLACEMENT >----- */
	SerializedProperty scalingAxis, anchor, joystickTouchSize;
	SerializedProperty customTouchSize_X, customTouchSize_Y;
	SerializedProperty customTouchSizePos_X, customTouchSizePos_Y;
	SerializedProperty dynamicPositioning;
	SerializedProperty joystickSize, radiusModifier;
	SerializedProperty customSpacing_X, customSpacing_Y;

	/* -----< STYLES AND OPTIONS >----- */
	SerializedProperty touchPad;
	SerializedProperty axis, boundary;
	SerializedProperty xDeadZone, yDeadZone, deadZoneOption;

	/* ------< SCRIPT REFERENCE >------ */
	SerializedProperty joystickName;
	SerializedProperty exposeValues;

	/* // ----< ANIMATED SECTIONS >---- \\ */
	AnimBool AssignedVariables, SizeAndPlacement;
	AnimBool StyleAndOptions, ScriptReference;

	// presets
	AnimBool AnimPresetDefault, AnimPresetTouchPad;
	AnimBool AnimPresetClassic;
	AnimBool CurrentPreset;

	/* // ----< ANIMATED VARIABLE >---- \\ */
	AnimBool customTouchSizeOption;
	AnimBool dzOneValueOption, dzTwoValueOption;
	AnimBool joystickNameUnassigned, joystickNameAssigned, exposeValuesTrue;

	public enum ScriptCast
	{
		Vector2,
		horizontalFloat,
		verticalFloat,
		getJoystickState
	}
	ScriptCast scriptCast;

	Canvas parentCanvas;


	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();
		
		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;

		if( PrefabUtility.GetPrefabType( Selection.activeGameObject ) != PrefabType.Prefab )
			return;

		parentCanvas = GetParentCanvas();
	}

	void OnDisable ()
	{
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	Canvas GetParentCanvas ()
	{
		if( Selection.activeGameObject == null )
			return null;

		// Store the current parent.
		Transform parent = Selection.activeGameObject.transform.parent;

		// Loop through parents as long as there is one.
		while( parent != null )
		{ 
			// If there is a Canvas component, return the component.
			if( parent.transform.GetComponent<Canvas>() )
				return parent.transform.GetComponent<Canvas>();
			
			// Else, shift to the next parent.
			parent = parent.transform.parent;
		}
		if( parent == null && PrefabUtility.GetPrefabType( Selection.activeGameObject ) != PrefabType.Prefab )
			SimpleJoystickCreator.RequestCanvas( Selection.activeGameObject );

		return null;
	}

	// Function called for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	// Function called to display an interactive header.
	void DisplayHeaderDropdown ( string headerName, string editorPref, AnimBool targetAnim )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		if( GUILayout.Button( EditorPrefs.GetBool( editorPref ) == true ? "Hide" : "Show", EditorStyles.miniButton, GUILayout.Width( 50 ), GUILayout.Height( 14f ) ) )
		{
			EditorPrefs.SetBool( editorPref, EditorPrefs.GetBool( editorPref ) == true ? false : true );
			targetAnim.target = EditorPrefs.GetBool( editorPref );
		}
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	// Function called to display a basic header.
	void DisplayHeader ( string headerName )
	{
		EditorGUILayout.BeginVertical( "Toolbar" );
		EditorGUILayout.LabelField( headerName, EditorStyles.boldLabel );
		EditorGUILayout.EndVertical();
	}

	bool CanvasErrors ()
	{
		// If the selection is actually the prefab within the Project window, then return no errors.
		if( PrefabUtility.GetPrefabType( Selection.activeGameObject ) == PrefabType.Prefab )
			return false;

		// If parentCanvas is unassigned, then get a new canvas and return no errors.
		if( parentCanvas == null )
		{
			parentCanvas = GetParentCanvas();
			return false;
		}

		// If the parentCanvas is not enabled, then return true for errors.
		if( parentCanvas.enabled == false )
			return true;

		// If the canvas' renderMode is not the needed one, then return true for errors.
		if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			return true;

		// If the canvas has a CanvasScaler component and it is not the correct option.
		if( parentCanvas.GetComponent<CanvasScaler>() && parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
			return true;

		return false;
	}

	/*
	For more information on the OnInspectorGUI and adding your own variables
	in the SimpleJoystick.cs script and displaying them in this script,
	see the EditorGUILayout section in the Unity Documentation to help out.
	*/
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		if( CanvasErrors() == true )
		{
			if( parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay )
			{
				EditorGUILayout.LabelField( "Canvas", EditorStyles.boldLabel );
				EditorGUILayout.HelpBox( "The parent Canvas needs to be set to 'Screen Space - Overlay' in order for the Simple Joystick to function correctly.", MessageType.Error );
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space( 5 );
				if( GUILayout.Button( "Update Canvas" ) )
				{
					parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
					parentCanvas = GetParentCanvas();
				}
				GUILayout.Space( 5 );
				if( GUILayout.Button( "Update Joystick" ) )
				{
					SimpleJoystickCreator.RequestCanvas( Selection.activeGameObject );
					parentCanvas = GetParentCanvas();
				}
				GUILayout.Space( 5 );
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			if( parentCanvas.GetComponent<CanvasScaler>() )
			{
				if( parentCanvas.GetComponent<CanvasScaler>().uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize )
				{
					EditorGUILayout.LabelField( "Canvas Scaler", EditorStyles.boldLabel );
					EditorGUILayout.HelpBox( "The Canvas Scaler component located on the parent Canvas needs to be set to 'Constant Pixel Size' in order for the Simple Joystick to function correctly.", MessageType.Error );
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space( 5 );
					if( GUILayout.Button( "Update Canvas" ) )
					{
						parentCanvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
						parentCanvas = GetParentCanvas();
						SimpleJoystick joystick = ( SimpleJoystick )target;
						joystick.UpdatePositioning();
					}
					GUILayout.Space( 5 );
					if( GUILayout.Button( "Update Joystick" ) )
					{
						SimpleJoystickCreator.RequestCanvas( Selection.activeGameObject );
						parentCanvas = GetParentCanvas();
					}
					GUILayout.Space( 5 );
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space();
				}
			}
			return;
		}

		// Store the joystick that we are selecting
		sj = ( SimpleJoystick )target;

		EditorGUILayout.Space();

		// --------------------------< PRESET >-------------------------- //
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( joystickPreset, new GUIContent( "Joystick Preset", "Determines what options are displayed below." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();

			AnimPresetDefault.target = sj.joystickPreset == SimpleJoystick.JoystickPreset.Default ? true : false;
			AnimPresetTouchPad.target = sj.joystickPreset == SimpleJoystick.JoystickPreset.TouchPad ? true : false;
			AnimPresetClassic.target = sj.joystickPreset == SimpleJoystick.JoystickPreset.Classic ? true : false;

			// Apply the changed preset option
			switch( sj.joystickPreset )
			{
				case SimpleJoystick.JoystickPreset.Default:
				{
					CurrentPreset = AnimPresetDefault;
					sj.touchPad = false;
					SetTouchPad();
					sj.joystickTouchSize = SimpleJoystick.JoystickTouchSize.Default;
					customTouchSizeOption.target = false;
					sj.dynamicPositioning = false;
				}break;
				case SimpleJoystick.JoystickPreset.TouchPad:
				{
					CurrentPreset = AnimPresetTouchPad;
					// Set all of the options to accepting the touch pad option now.
					sj.touchPad = true;
					SetTouchPad();
					sj.joystickTouchSize = SimpleJoystick.JoystickTouchSize.Custom;
				}
				break;
				case SimpleJoystick.JoystickPreset.Classic:
				{
					CurrentPreset = AnimPresetClassic;
					// Set all of the options to accepting the touch pad option now.
					sj.touchPad = false;
					sj.dynamicPositioning = false;
					SetTouchPad();
					sj.joystickTouchSize = SimpleJoystick.JoystickTouchSize.Default;
					sj.scalingAxis = SimpleJoystick.ScalingAxis.Height;
				}
				break;
				default:
				{

				}break;
			}
		}
		// ------------------------< END PRESET >------------------------ //

		EditorGUILayout.Space();

		#region ASSIGNED VARIABLES
		/* ----------------------------------------< ** ASSIGNED VARIABLES ** >---------------------------------------- */
		DisplayHeaderDropdown( "Assigned Variables", "UUI_Variables", AssignedVariables );
		if( EditorGUILayout.BeginFadeGroup( AssignedVariables.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystick );
			EditorGUILayout.PropertyField( joystickSizeFolder, new GUIContent( "Size Folder" ) );
			EditorGUILayout.PropertyField( joystickBase );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END ASSIGNED VARIABLES ** >-------------------------------------- */
		#endregion

		EditorGUILayout.Space();

		#region SCRIPT REFERENCE
		/* ------------------------------------------< ** SCRIPT REFERENCE ** >------------------------------------------- */
		DisplayHeaderDropdown( "Script Reference", "UUI_ScriptReference", ScriptReference );
		if( EditorGUILayout.BeginFadeGroup( ScriptReference.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( joystickName );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				if( sj.joystickName == string.Empty )
				{
					joystickNameUnassigned.target = true;
					joystickNameAssigned.target = false;
				}
				else
				{
					joystickNameUnassigned.target = false;
					joystickNameAssigned.target = true;
				}
			}

			if( EditorGUILayout.BeginFadeGroup( joystickNameUnassigned.faded ) )
			{
				EditorGUILayout.HelpBox( "Please assign a Joystick Name in order to be able to get this joystick's position dynamically.", MessageType.Warning );
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			if( EditorGUILayout.BeginFadeGroup( joystickNameAssigned.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( " Script Reference:", EditorStyles.boldLabel );
				scriptCast = ( ScriptCast )EditorGUILayout.EnumPopup( "Joystick Use: ", scriptCast );
				if( scriptCast == ScriptCast.Vector2 )
					EditorGUILayout.TextField( "SimpleJoystick.GetPosition( \"" + sj.joystickName + "\" )" );
				else if( scriptCast == ScriptCast.horizontalFloat )
					EditorGUILayout.TextField( "SimpleJoystick.GetPosition( \"" + sj.joystickName + "\" ).x" );
				else if( scriptCast == ScriptCast.verticalFloat )
					EditorGUILayout.TextField( "SimpleJoystick.GetPosition( \"" + sj.joystickName + "\" ).y" );
				else
					EditorGUILayout.TextField( "SimpleJoystick.GetJoystickState( \"" + sj.joystickName + "\" )" );
				GUILayout.Space( 5 );
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( exposeValues );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				exposeValuesTrue.target = sj.exposeValues;
			}

			if( EditorGUILayout.BeginFadeGroup( exposeValuesTrue.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( " Current Position:", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUILayout.LabelField( "Horizontal Value: " + sj.horizontalValue.ToString( "F2" ) );
				EditorGUILayout.LabelField( "Vertical Value: " + sj.verticalValue.ToString( "F2" ) );
				EditorGUI.indentLevel = 0;
				EditorGUILayout.EndVertical();
			}
			if( ScriptReference.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
		}
		EditorGUILayout.EndFadeGroup();
		/* -----------------------------------------< ** END SCRIPT REFERENCE ** >---------------------------------------- */
		#endregion
		
		EditorGUILayout.Space();

		EditorGUILayout.BeginFadeGroup( CurrentPreset.faded );
		{
			switch( sj.joystickPreset )
			{
				case SimpleJoystick.JoystickPreset.Default:
				{
					PresetDefault();
				}break;
				case SimpleJoystick.JoystickPreset.TouchPad:
				{
					PresetTouchPad();
				}break;
				case SimpleJoystick.JoystickPreset.Classic:
				{
					PresetClassic();
				}break;
				default:
				{
					PresetDefault();
				}break;
			}
		}
		EditorGUILayout.EndFadeGroup();

		EditorGUILayout.Space();

		/* ----------------------------------------------< ** HELP TIPS ** >---------------------------------------------- */
		if( sj.joystick == null )
			EditorGUILayout.HelpBox( "Joystick needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		if( sj.joystickSizeFolder == null )
			EditorGUILayout.HelpBox( "Joystick Size Folder needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		if( sj.joystickBase == null )
			EditorGUILayout.HelpBox( "Joystick Base needs to be assigned in 'Assigned Variables'!", MessageType.Error );
		/* -------------------------------------------< ** END HELP TIPS ** >-------------------------------------------- */

		Repaint();
	}

	void PresetDefault ()
	{
		/* ----------------------------------------< ** SIZE AND PLACEMENT ** >---------------------------------------- */
		DisplayHeaderDropdown( "Size and Placement", "UUI_SizeAndPlacement", SizeAndPlacement );
		if( EditorGUILayout.BeginFadeGroup( SizeAndPlacement.faded ) )
		{
			EditorGUILayout.Space();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( scalingAxis, new GUIContent( "Scaling Axis", "The axis to scale the joystick from." ) );
			EditorGUILayout.PropertyField( anchor, new GUIContent( "Anchor", "The side of the screen that the joystick will be anchored to." ) );
			EditorGUILayout.PropertyField( joystickTouchSize, new GUIContent( "Touch Size", "The size of the area in which the touch can be initiated." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();

				customTouchSizeOption.target = sj.joystickTouchSize == SimpleJoystick.JoystickTouchSize.Custom ? true : false;
			}
			if( EditorGUILayout.BeginFadeGroup( customTouchSizeOption.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.LabelField( "Touch Size Customization", EditorStyles.boldLabel );
				EditorGUI.indentLevel = 1;
				EditorGUI.BeginChangeCheck();
				{
					EditorGUILayout.Slider( customTouchSize_X, 0.0f, 100.0f, new GUIContent( "Width", "The width of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSize_Y, 0.0f, 100.0f, new GUIContent( "Height", "The height of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSizePos_X, 0.0f, 100.0f, new GUIContent( "X Position", "The horizontal position of the Joystick Touch Area." ) );
					EditorGUILayout.Slider( customTouchSizePos_Y, 0.0f, 100.0f, new GUIContent( "Y Position", "The vertical position of the Joystick Touch Area." ) );
				}
				if( EditorGUI.EndChangeCheck() )
					serializedObject.ApplyModifiedProperties();
				GUILayout.Space( 1 );
				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( SizeAndPlacement.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( dynamicPositioning, new GUIContent( "Dynamic Positioning", "Moves the joystick to the position of the initial touch." ) );
			EditorGUILayout.Slider( joystickSize, 1.0f, 3.0f, new GUIContent( "Joystick Size", "The overall size of the joystick." ) );
			EditorGUILayout.Slider( radiusModifier, 2.0f, 7.0f, new GUIContent( "Radius", "Determines how far the joystick can move visually from the center." ) );
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.LabelField( " Joystick Position", EditorStyles.boldLabel );
			EditorGUI.indentLevel = 1;
			EditorGUILayout.Slider( customSpacing_X, 0.0f, 50.0f, new GUIContent( "X Position:", "The horizontal position of the joystick." ) );
			EditorGUILayout.Slider( customSpacing_Y, 0.0f, 100.0f, new GUIContent( "Y Position:", "The vertical position of the joystick." ) );
			EditorGUI.indentLevel = 0;
			GUILayout.Space( 1 );
			EditorGUILayout.EndVertical();
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
		}
		if( AnimPresetDefault.faded == 1 )
			EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END SIZE AND PLACEMENT ** >-------------------------------------- */

		EditorGUILayout.Space();
		
		/* ------------------------------------------< ** STYLE AND OPTIONS ** >----------------------------------------- */
		DisplayHeaderDropdown( "Style and Options", "UUI_StyleAndOptions", StyleAndOptions );
		if( EditorGUILayout.BeginFadeGroup( StyleAndOptions.faded ) )
		{
			EditorGUILayout.Space();
			
			// --------------------------< TOUCH PAD >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( touchPad, new GUIContent( "Touch Pad", "Disables the visuals of the joystick." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				SetTouchPad();
			}
			
			if( sj.touchPad == true && sj.joystickBase == null )
				EditorGUILayout.HelpBox( "Joystick Base needs to be assigned in the Assigned Variables section.", MessageType.Error );
			// ------------------------< END TOUCH PAD >------------------------ //
			
			// -----------------------------< AXIS >---------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( axis, new GUIContent( "Axis", "Constrains the joystick to a certain axis." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// ---------------------------< END AXIS >-------------------------- //
			
			// ---------------------------< BOUNDARY >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( boundary, new GUIContent( "Boundary", "Determines how the joystick's position is clamped." ) );
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// -------------------------< END BOUNDARY >------------------------ //
			
			// --------------------------< DEAD ZONE >-------------------------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( deadZoneOption, new GUIContent( "Dead Zone", "Forces the joystick position to being only values of -1, 0, and 1." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				dzOneValueOption.target = sj.deadZoneOption == SimpleJoystick.DeadZoneOption.OneValue ? true : false;
				dzTwoValueOption.target = sj.deadZoneOption == SimpleJoystick.DeadZoneOption.TwoValues ? true : false;
			}
			EditorGUI.indentLevel = 1;
			EditorGUI.BeginChangeCheck();
			if( EditorGUILayout.BeginFadeGroup( dzTwoValueOption.faded ) )
			{
				EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "X Dead Zone", "X values within this range will be forced to 0." ) );
				EditorGUILayout.Slider( yDeadZone, 0.0f, 1.0f, new GUIContent( "Y Dead Zone", "Y values within this range will be forced to 0." ) );
			}
			if( StyleAndOptions.faded == 1 && AnimPresetDefault.faded == 1 )
				EditorGUILayout.EndFadeGroup();
			
			if( EditorGUILayout.BeginFadeGroup( dzOneValueOption.faded ) )
			{
				EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "Dead Zone", "Values within this range will be forced to 0." ) );
				sj.yDeadZone = sj.xDeadZone;
			}
			if( StyleAndOptions.faded == 1 && AnimPresetDefault.faded == 1 )
				EditorGUILayout.EndFadeGroup();

			EditorGUI.indentLevel = 0;
			if( EditorGUI.EndChangeCheck() )
				serializedObject.ApplyModifiedProperties();
			// ------------------------< END DEAD ZONE >------------------------ //
		}
		if( AnimPresetDefault.faded == 1 )
			EditorGUILayout.EndFadeGroup();
		/* --------------------------------------< ** END STYLE AND OPTIONS ** >------------------------------------- */
	}

	void PresetTouchPad ()
	{
		DisplayHeader( "Touch Pad Options" );
		EditorGUILayout.BeginVertical( "Box" );
		EditorGUILayout.LabelField( "Touch Pad Area" );
		EditorGUI.indentLevel = 1;
		EditorGUI.BeginChangeCheck();
		{
			EditorGUILayout.Slider( customTouchSize_X, 0.0f, 100.0f, new GUIContent( "Width", "The width of the touch area." ) );
			EditorGUILayout.Slider( customTouchSize_Y, 0.0f, 100.0f, new GUIContent( "Height", "The height of the touch area." ) );
			EditorGUILayout.Slider( customTouchSizePos_X, 0.0f, 100.0f, new GUIContent( "X Position", "The horizontal position of the touch area." ) );
			EditorGUILayout.Slider( customTouchSizePos_Y, 0.0f, 100.0f, new GUIContent( "Y Position", "The vertical position of the touch area." ) );
		}
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndVertical();
		EditorGUI.indentLevel = 0;

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.Slider( radiusModifier, 2.0f, 7.0f, new GUIContent( "Radius", "Determines how far the joystick can move visually from the center." ) );
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();

		// -----------------------------< AXIS >---------------------------- //
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( axis, new GUIContent( "Axis", "Constrains the joystick to a certain axis." ) );
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();
		// ---------------------------< END AXIS >-------------------------- //
			
		// --------------------------< DEAD ZONE >-------------------------- //
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( deadZoneOption, new GUIContent( "Dead Zone", "Forces the joystick position to being only values of -1, 0, and 1." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			dzOneValueOption.target = sj.deadZoneOption == SimpleJoystick.DeadZoneOption.OneValue ? true : false;
			dzTwoValueOption.target = sj.deadZoneOption == SimpleJoystick.DeadZoneOption.TwoValues ? true : false;
		}
		EditorGUI.indentLevel = 1;
		EditorGUI.BeginChangeCheck();
		if( EditorGUILayout.BeginFadeGroup( dzTwoValueOption.faded ) )
		{
			EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "X Dead Zone", "X values within this range will be forced to 0." ) );
			EditorGUILayout.Slider( yDeadZone, 0.0f, 1.0f, new GUIContent( "Y Dead Zone", "Y values within this range will be forced to 0." ) );
		}
		if( AnimPresetTouchPad.faded == 1 )
			EditorGUILayout.EndFadeGroup();
			
		if( EditorGUILayout.BeginFadeGroup( dzOneValueOption.faded ) )
		{
			EditorGUILayout.Slider( xDeadZone, 0.0f, 1.0f, new GUIContent( "Dead Zone", "Values within this range will be forced to 0." ) );
			sj.yDeadZone = sj.xDeadZone;
		}
		if( AnimPresetTouchPad.faded == 1 )
			EditorGUILayout.EndFadeGroup();

		EditorGUI.indentLevel = 0;
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();
		// ------------------------< END DEAD ZONE >------------------------ //
	}

	void PresetClassic ()
	{
		DisplayHeader( "Classic Options" );
		EditorGUILayout.Space();
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( anchor, new GUIContent( "Anchor", "The side of the screen that the joystick will be anchored to." ) );
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.Slider( joystickSize, 1.0f, 3.0f, new GUIContent( "Joystick Size", "The overall size of the joystick." ) );
		EditorGUILayout.Slider( radiusModifier, 2.0f, 7.0f, new GUIContent( "Radius", "Determines how far the joystick can move visually from the center." ) );
		EditorGUILayout.BeginVertical( "Box" );
		EditorGUILayout.LabelField( "Joystick Position" );
		EditorGUI.indentLevel = 1;
		EditorGUILayout.Slider( customSpacing_X, 0.0f, 50.0f, new GUIContent( "X Position:", "The horizontal position of the joystick." ) );
			EditorGUILayout.Slider( customSpacing_Y, 0.0f, 100.0f, new GUIContent( "Y Position:", "The vertical position of the joystick." ) );
		EditorGUI.indentLevel = 0;
		EditorGUILayout.EndVertical();
		if( EditorGUI.EndChangeCheck() )
			serializedObject.ApplyModifiedProperties();
	}

	// This function stores the references to the variables of the target.
	void StoreReferences ()
	{
		if( serializedObject == null )
			return;

		sj = ( SimpleJoystick ) target;

		/* -----< ASSIGNED VARIABLES >----- */
		joystick = serializedObject.FindProperty( "joystick" );
		joystickSizeFolder = serializedObject.FindProperty( "joystickSizeFolder" );
		joystickBase = serializedObject.FindProperty( "joystickBase" );
		
		/* -----< SIZE AND PLACEMENT >----- */
		scalingAxis = serializedObject.FindProperty( "scalingAxis" );
		anchor = serializedObject.FindProperty( "anchor" );
		joystickTouchSize = serializedObject.FindProperty( "joystickTouchSize" );
		customTouchSize_X = serializedObject.FindProperty( "customTouchSize_X" );
		customTouchSize_Y = serializedObject.FindProperty( "customTouchSize_Y" );
		customTouchSizePos_X = serializedObject.FindProperty( "customTouchSizePos_X" );
		customTouchSizePos_Y = serializedObject.FindProperty( "customTouchSizePos_Y" );
		dynamicPositioning = serializedObject.FindProperty( "dynamicPositioning" );
		joystickSize = serializedObject.FindProperty( "joystickSize" );
		radiusModifier = serializedObject.FindProperty( "radiusModifier" );
		customSpacing_X = serializedObject.FindProperty( "customSpacing_X" );
		customSpacing_Y = serializedObject.FindProperty( "customSpacing_Y" );
		
		/* -----< STYLES AND OPTIONS >----- */
		touchPad = serializedObject.FindProperty( "touchPad" );
		axis = serializedObject.FindProperty( "axis" );
		boundary = serializedObject.FindProperty( "boundary" );
		deadZoneOption = serializedObject.FindProperty( "deadZoneOption" );
		xDeadZone = serializedObject.FindProperty( "xDeadZone" );
		yDeadZone = serializedObject.FindProperty( "yDeadZone" );
		joystickPreset = serializedObject.FindProperty( "joystickPreset" );

		/* ------< SCRIPT REFERENCE >------ */
		joystickName = serializedObject.FindProperty( "joystickName" );
		exposeValues = serializedObject.FindProperty( "exposeValues" );
		
		/* // ----< ANIMATED SECTIONS >---- \\ */
		AssignedVariables = new AnimBool( EditorPrefs.GetBool( "UUI_Variables" ) );
		SizeAndPlacement = new AnimBool( EditorPrefs.GetBool( "UUI_SizeAndPlacement" ) );
		StyleAndOptions = new AnimBool( EditorPrefs.GetBool( "UUI_StyleAndOptions" ) );
		ScriptReference = new AnimBool( EditorPrefs.GetBool( "UUI_ScriptReference" ) );

		AnimPresetDefault = new AnimBool( sj.joystickPreset == SimpleJoystick.JoystickPreset.Default ? true : false );
		AnimPresetTouchPad = new AnimBool( sj.joystickPreset == SimpleJoystick.JoystickPreset.TouchPad ? true : false );
		AnimPresetClassic = new AnimBool( sj.joystickPreset == SimpleJoystick.JoystickPreset.Classic ? true : false );

		if( sj.joystickPreset == SimpleJoystick.JoystickPreset.Default )
			CurrentPreset = AnimPresetDefault;
		else if( sj.joystickPreset == SimpleJoystick.JoystickPreset.TouchPad )
			CurrentPreset = AnimPresetTouchPad;
		else if( sj.joystickPreset == SimpleJoystick.JoystickPreset.Classic )
			CurrentPreset = AnimPresetClassic;
		else
			CurrentPreset = AnimPresetDefault;//< --- Change this
		
		/* // ----< ANIMATED VARIABLES >---- \\ */
		customTouchSizeOption = new AnimBool( sj.joystickTouchSize == SimpleJoystick.JoystickTouchSize.Custom ? true : false );
		dzOneValueOption = new AnimBool( sj.deadZoneOption == SimpleJoystick.DeadZoneOption.OneValue ? true : false );
		dzTwoValueOption = new AnimBool( sj.deadZoneOption == SimpleJoystick.DeadZoneOption.TwoValues ? true : false );

		joystickNameUnassigned = new AnimBool( sj.joystickName != string.Empty ? false : true );
		joystickNameAssigned = new AnimBool( sj.joystickName != string.Empty ? true : false );
		exposeValuesTrue = new AnimBool( sj.exposeValues == true ? true : false );

		SetTouchPad();
	}

	void SetTouchPad ()
	{
		if( sj.touchPad == true )
		{
			if( sj.dynamicPositioning == false )
				sj.dynamicPositioning = true;

			if( sj.joystickBase != null && sj.joystickBase.GetComponent<Image>().enabled == true )
				sj.joystickBase.GetComponent<Image>().enabled = false;
				
			if( sj.joystick.GetComponent<Image>().enabled == true )
				sj.joystick.GetComponent<Image>().enabled = false;
		}
		else
		{
			if( sj.joystickBase != null )
			{
				if( sj.joystickBase.GetComponent<Image>().enabled == false )
					sj.joystickBase.GetComponent<Image>().enabled = true;
			}
			if( sj.joystick.GetComponent<Image>().enabled == false )
				sj.joystick.GetComponent<Image>().enabled = true;
		}
	}
}

/* Written by Kaz Crowe */
/* SimpleJoystickCreator.cs */
public class SimpleJoystickCreator
{
	[MenuItem( "GameObject/UI/Simple Joystick", false, 0 )]
	private static void CreateSimpleJoystick ()
	{
		// Find and load our joystick game object
		GameObject joystickPrefab = EditorGUIUtility.Load( "Simple Joystick/SimpleJoystick.prefab" ) as GameObject;

		// if we found our SimpleJoystick, then create it in our scene
		if( joystickPrefab == null )
		{
			Debug.LogError( "Could not find 'SimpleJoystick.prefab' in the Editor Default Resources folder." );
			return;
		}
		CreateNewUI( joystickPrefab );
	}

	private static void CreateNewUI ( Object objectPrefab )
	{
		GameObject prefab = ( GameObject )Object.Instantiate( objectPrefab, Vector3.zero, Quaternion.identity );
		prefab.name = objectPrefab.name;
		Selection.activeGameObject = prefab;
		RequestCanvas( prefab );
	}

	private static void CreateNewCanvas ( GameObject child )
	{
		GameObject root = new GameObject( "Canvas" );
		root.layer = LayerMask.NameToLayer( "UI" );
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo( root, "Create " + root.name );

		child.transform.SetParent( root.transform, false );
		
		CreateEventSystem();
	}

	private static void CreateEventSystem ()
	{
		Object esys = Object.FindObjectOfType<EventSystem>();
		if( esys == null )
		{
			GameObject eventSystem = new GameObject( "EventSystem" );
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();
			
			Undo.RegisterCreatedObjectUndo( eventSystem, "Create " + eventSystem.name );
		}
	}

	/* PUBLIC STATIC FUNCTIONS */
	public static void RequestCanvas ( GameObject child )
	{
		Canvas[] allCanvas = Object.FindObjectsOfType( typeof( Canvas ) ) as Canvas[];

		for( int i = 0; i < allCanvas.Length; i++ )
		{
			if( allCanvas[ i ].renderMode == RenderMode.ScreenSpaceOverlay && allCanvas[ i ].enabled == true && !allCanvas[ i ].GetComponent<CanvasScaler>() )
			{
				child.transform.SetParent( allCanvas[ i ].transform, false );
				CreateEventSystem();
				return;
			}
		}
		CreateNewCanvas( child );
	}
}