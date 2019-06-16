/* Written by Kaz Crowe */
/* SimpleJoystickWindow.cs */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SimpleJoystickWindow : EditorWindow
{
	static string version = "1.3.0";// ALWAYS UPDATE
	static int importantChanges = 1;// UPDATE ON IMPORTANT CHANGES
	static string menuTitle = "Main Menu";

	// LAYOUT STYLES //
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();

	GUILayoutOption[] buttonSize = new GUILayoutOption[] { GUILayout.Width( 200 ), GUILayout.Height( 35 ) };
	GUILayoutOption[] docSize = new GUILayoutOption[] { GUILayout.Width( 300 ), GUILayout.Height( 330 ) };
	GUISkin style;

	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Main Menu" };
	static PageInformation howTo = new PageInformation() { pageName = "How To" };
	static PageInformation overview = new PageInformation() { pageName = "Overview" };
	static PageInformation documentation = new PageInformation() { pageName = "Documentation" };
	static PageInformation extras = new PageInformation() { pageName = "Extras" };
	static PageInformation otherProducts = new PageInformation() { pageName = "Other Products" };
	static PageInformation feedback = new PageInformation() { pageName = "Feedback" };
	static PageInformation changeLog = new PageInformation() { pageName = "Change Log" };
	static PageInformation futureUpdates = new PageInformation() { pageName = "Future Updates" };
	static PageInformation versionChanges = new PageInformation() { pageName = "Version Changes" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();

	Texture2D scriptReference, positionVisual;
	Texture2D ubPromo, usbPromo;


	[MenuItem( "Window/Tank and Healer Studio/Simple Joystick", false, 1 )]
	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<SimpleJoystickWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 500, 500 );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	void OnEnable ()
	{
		style = ( GUISkin )EditorGUIUtility.Load( "Simple Joystick/SimpleJoystickEditorSkin.guiskin" );
		
		ubPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/UB_Promo.png" );
		usbPromo = ( Texture2D )EditorGUIUtility.Load( "Ultimate UI/USB_Promo.png" );
		scriptReference = ( Texture2D ) EditorGUIUtility.Load( "Simple Joystick/ScriptReference.jpg" );
		positionVisual = ( Texture2D )EditorGUIUtility.Load( "Simple Joystick/SJ_PosVisual.png" );
		
		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainMenu;
		howTo.targetMethod = HowTo;
		overview.targetMethod = Overview;
		documentation.targetMethod = Documentation;
		extras.targetMethod = Extras;
		otherProducts.targetMethod = OtherProducts;
		feedback.targetMethod = Feedback;
		changeLog.targetMethod = ChangeLog;
		futureUpdates.targetMethod = FutureUpdates;
		versionChanges.targetMethod = VersionChanges;
		thankYou.targetMethod = ThankYou;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;
	}
	
	void OnGUI ()
	{
		if( style == null )
		{
			GUILayout.BeginVertical( "Box" );
			GUILayout.FlexibleSpace();
			ErrorScreen();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			return;
		}

		GUI.skin = style;

		paragraphStyle = GUI.skin.GetStyle( "ParagraphStyle" );
		itemHeaderStyle = GUI.skin.GetStyle( "ItemHeader" );
		sectionHeaderStyle = GUI.skin.GetStyle( "SectionHeader" );
		
		EditorGUILayout.Space();

		GUILayout.BeginVertical( "Box" );
		
		EditorGUILayout.LabelField( "Simple Joystick", GUI.skin.GetStyle( "WindowTitle" ) );

		GUILayout.Space( 3 );
		
		if( GUILayout.Button( "Version " + version, GUI.skin.GetStyle( "VersionNumber" ) ) && currentPage != changeLog )
			NavigateForward( changeLog );

		GUILayout.Space( 12 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 5 );
		if( pageHistory.Count > 1 )
		{
			if( GUILayout.Button( "", GUI.skin.GetStyle( "BackButton" ), GUILayout.Width( 80 ), GUILayout.Height( 40 ) ) )
				NavigateBack();
		}
		else
			GUILayout.Space( 80 );

		GUILayout.Space( 15 );
		EditorGUILayout.LabelField( menuTitle, GUI.skin.GetStyle( "MenuTitle" ) );
		GUILayout.FlexibleSpace();
		GUILayout.Space( 80 );
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		if( currentPage.targetMethod != null )
			currentPage.targetMethod();

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Space( 25 );

		EditorGUILayout.EndVertical();

		Repaint();
	}

	void ErrorScreen ()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "ERROR", EditorStyles.boldLabel );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "Could not find the needed GUISkin located in the Editor Default Resources folder. Please ensure that the correct GUISkin, SimpleJoystickEditorSkin, is in the right folder( Editor Default Resources/Simple Joystick ) before trying to access the Simple Joystick Window.", EditorStyles.wordWrappedLabel );
		GUILayout.Space( 50 );
		EditorGUILayout.EndHorizontal();
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];
	}

	static void NavigateForward ( PageInformation menu )
	{
		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;
	}
	
	void MainMenu ()
	{
		mainMenu.scrollPosition = EditorGUILayout.BeginScrollView( mainMenu.scrollPosition, false, false, docSize );

		GUILayout.Space( 25 );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "How To", buttonSize ) )
			NavigateForward( howTo );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Overview", buttonSize ) )
			NavigateForward( overview );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Documentation", buttonSize ) )
			NavigateForward( documentation );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Extras", buttonSize ) )
			NavigateForward( extras );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Other Products", buttonSize ) )
			NavigateForward( otherProducts );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Feedback", buttonSize ) )
			NavigateForward( feedback );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndScrollView();
	}
	
	void HowTo ()
	{
		howTo.scrollPosition = EditorGUILayout.BeginScrollView( howTo.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   To create a Simple Joystick in your scene, just go up to GameObject / UI / Simple Joystick. What this does is locates the Simple Joystick prefab that is located within the Editor Default Resources folder, and creates a Simple Joystick within the scene.\n\nThis method of adding a Simple Joystick to your scene ensures that the Joystick will have a Canvas and an EventSystem so that it can work correctly.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   One of the great things about the Simple Joystick is how easy it is to reference to other scripts. The first thing that you will want to make sure to do is to name the joystick in the Script Reference section. After this is complete, you will be able to reference that particular joystick by it's name from a static function within the Simple Joystick script.\n\nAfter the joystick has been given a name in the Script Reference section, we can get that joystick's position by creating a Vector2 variable at run time and storing the result from the static function: 'GetPosition'. This Vector2 will be the joystick's position, and will contain values between -1, and 1, with 0 being at the center.\n\nKeep in mind that the joystick's left and right ( horizontal ) movement is translated into this Vector2's X value, while the up and down ( vertical ) is the Vector2's Y value. This will be important when applying the Simple Joystick's position to your scripts.", EditorStyles.wordWrappedLabel );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 10 );
		GUILayout.Label( positionVisual, GUILayout.Width( 200 ), GUILayout.Height( 200 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Example", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   Let's assume that we want to use a joystick for a characters movement. The first thing to do is to assign the name \"Movement\" in the Joystick Name variable located in the Script Reference section of the Simple Joystick.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( scriptReference );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "After that, we need to create a Vector2 variable to store the result of the joystick's position returned by the 'GetPosition' function. In order to get the \"Movement\" joystick's position, we need to pass in the name \"Movement\" as the argument.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.TextArea( "Vector2 joystickPosition = SimpleJoystick.GetPosition( \"Movement\" );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "After this, the joystickPosition variable can be used in anything that needs joystick input. For example, if you are wanting to put the joystick's position into a character movement script, you would create a Vector3 variable for movement direction, and put in the appropriate value of the Simple Joystick's position.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.TextArea( "Vector3 movementDirection = new Vector3( joystickPosition.x, 0, joystickPosition.y );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "In the above example, the joystickPosition variable is used to give the movement direction values in the X and Z directions. This is because you generally don't want your character to move in the Y direction unless the user jumps. That is why we put the joystickPosition.y into the Z value of the movementDirection variable.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Understanding how to use the values from any input is important when creating character controllers, so experiment with the values and try to understand how the mobile input can be used in different ways.", EditorStyles.wordWrappedLabel );

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndScrollView();
	}
	
	void Overview ()
	{
		overview.scrollPosition = EditorGUILayout.BeginScrollView( overview.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Assigned Variables", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   In the Assigned Variables section, there are three components that should already be assigned if you are using one of the Prefabs that has been provided. If not, you will see error messages on the Simple Joystick inspector that will help you to see if any of these variables are left unassigned. Please note that these need to be assigned in order for the Simple Joystick to work properly.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < SIZE AND PLACEMENT > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Size And Placement", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Size and Placement section allows you to customize the joystick's size and placement on the screen, as well as determine where the user's touch can be processed for the selected joystick.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );

		// Scaling Axis
		EditorGUILayout.LabelField( "Scaling Axis", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Determines which axis the joystick will be scaled from. If Height is chosen, then the joystick will scale itself proportionately to the Height of the screen.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );

		// Anchor
		EditorGUILayout.LabelField( "Anchor", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Determines which side of the screen that the joystick will be anchored to.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( paragraphSpace );

		// Touch Size
		EditorGUILayout.LabelField( "Touch Size", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Touch Size configures the size of the area where the user can touch. You have the options of either 'Default','Medium', 'Large' or 'Custom'. When the option 'Custom' is selected, an additional box will be displayed that allows for a more adjustable touch area.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( paragraphSpace );

		// Touch Size Customization
		EditorGUILayout.LabelField( "Touch Size Customization", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "If the 'Custom' option of the Touch Size is selected, then you will be presented with the Touch Size Customization box. Inside this box are settings for the Width and Height of the touch area, as well as the X and Y position of the touch area on the screen.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( paragraphSpace );

		// Dynamic Positioning
		EditorGUILayout.LabelField( "Dynamic Positioning", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Dynamic Positioning will make the joystick snap to where the user touches, instead of the user having to touch a direct position to get the joystick. The Touch Size option will directly affect the area where the joystick can snap to.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( paragraphSpace );

		// Joystick Size
		EditorGUILayout.LabelField( "Joystick Size", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Joystick Size will change the scale of the joystick. Since everything is calculated out according to screen size, your joystick Touch Size and other properties will scale proportionately with the joystick's size along your specified Scaling Axis.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( paragraphSpace );

		// Radius
		EditorGUILayout.LabelField( "Radius", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Radius determines how far away the joystick will move from center when it is being used, and will scale proportionately with the joystick.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( paragraphSpace );

		// Joystick Position
		EditorGUILayout.LabelField( "Joystick Position", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Joystick Position will present you with two sliders. The X value will determine how far the Joystick is away from the Left and Right sides of the screen, and the Y value from the Top and Bottom. This will encompass 50% of your screen, relevant to your Anchor selection.", EditorStyles.wordWrappedLabel );
		/* \\\\ -------------------------- < END SIZE AND PLACEMENT > --------------------------- //// */

		GUILayout.Space( sectionSpace );

		/* //// ----------------------------- < STYLE AND OPTIONS > ----------------------------- \\\\ */
		EditorGUILayout.LabelField( "Style And Options", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The Style and Options section contains options that effect how the joystick handles and is visually presented to the user.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( paragraphSpace );

		// Touch Pad
		EditorGUILayout.LabelField( "Touch Pad", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Touch Pad presents you with the option to disable the visuals of the joystick, whilst keeping all functionality. When paired with Dynamic Positioning and Throwable, this option can give you a very smooth camera control.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( paragraphSpace );

		// Axis
		EditorGUILayout.LabelField( "Axis", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Axis determines which axis the joystick will snap to. By default it is set to Both, which means the joystick will use both the X and Y axis for movement. If either the X or Y option is selected, then the joystick will snap to the corresponding axis.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( paragraphSpace );

		// Boundary
		EditorGUILayout.LabelField( "Boundary", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Boundary will allow you to decide if you want to have a square or circular edge to your joystick.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( paragraphSpace );

		// Dead Zone
		EditorGUILayout.LabelField( "Dead Zone", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( "Dead Zone gives the option of setting one or two values that the joystick is constrained by. When selected, the joystick will be forced to a maximum value when it has past the set dead zone.", EditorStyles.wordWrappedLabel );
		/* //// --------------------------- < END STYLE AND OPTIONS > --------------------------- \\\\ */

		EditorGUILayout.EndScrollView();
	}
	
	void Documentation ()
	{
		documentation.scrollPosition = EditorGUILayout.BeginScrollView( documentation.scrollPosition, false, false, docSize );
		
		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		// Vector2 JoystickPosition
		EditorGUILayout.LabelField( "Vector2 GetPosition", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the Simple Joystick's position in a Vector2 variable. The X value that is returned represents the Left and Right( Horizontal ) movement of the joystick, whereas the Y value represents the Up and Down( Vertical ) movement of the joystick. The values returned will always be between -1 and 1, with 0 being the center.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );
		
		// float GetHorizontalAxis
		EditorGUILayout.LabelField( "float GetHorizontalAxis", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current horizontal value of the joystick's position. The value returned will always be between -1 and 1, with 0 being the neutral position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// float GetVerticalAxis
		EditorGUILayout.LabelField( "float GetVerticalAxis", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current vertical value of the joystick's position. The value returned will always be between -1 and 1, with 0 being the neutral position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// float GetDistance
		EditorGUILayout.LabelField( "float GetDistance", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the distance of the joystick from it's center in a float value. The value returned will always be a value between 0 and 1.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );

		// UpdatePositioning()
		EditorGUILayout.LabelField( "UpdatePositioning()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Updates the size and positioning of the Simple Joystick. This function can be used to update any options that may have been changed prior to Start().", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( paragraphSpace );

		// ResetJoystick()
		EditorGUILayout.LabelField( "ResetJoystick()", itemHeaderStyle );
		EditorGUILayout.LabelField( "Resets the joystick back to it's neutral state.", EditorStyles.wordWrappedLabel );
				
		GUILayout.Space( paragraphSpace );

		// DisableJoystick()
		EditorGUILayout.LabelField( "DisableJoystick()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function will reset the Simple Joystick and disable the gameObject. Use this function when wanting to disable the Simple Joystick from being used.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );

		// EnableJoystick()
		EditorGUILayout.LabelField( "EnableJoystick()", itemHeaderStyle );
		EditorGUILayout.LabelField( "This function will ensure that the Simple Joystick is completely reset before enabling itself to be used again.", paragraphStyle );
						
		GUILayout.Space( paragraphSpace );

		// GetJoystickState()
		EditorGUILayout.LabelField( "bool GetJoystickState", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the state that the joystick is currently in. This function will return true when the joystick is being interacted with, and false when not.", EditorStyles.wordWrappedLabel );
		
		GUILayout.Space( sectionSpace );
		
		/* //// --------------------------- < STATIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Static Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		// SimpleJoystick.GetSimpleJoystick
		EditorGUILayout.LabelField( "Vector2 SimpleJoystick.GetSimpleJoystick( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the Simple Joystick that has been registered with the referenced joystick name.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );

		// SimpleJoystick.GetPosition
		EditorGUILayout.LabelField( "Vector2 SimpleJoystick.GetPosition( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the Simple Joystick's position in a Vector2 variable. This static function will return the same exact value as the public GetPosition function. See GetPosition above for more information.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );
		
		// float GetHorizontalAxis
		EditorGUILayout.LabelField( "float GetHorizontalAxis( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current horizontal value of the joystick's position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// float GetVerticalAxis
		EditorGUILayout.LabelField( "float GetVerticalAxis( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the current vertical value of the joystick's position.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		// SimpleJoystick.GetDistance
		EditorGUILayout.LabelField( "float SimpleJoystick.GetDistance( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the distance of the joystick from it's center in a float value. This static function will return the same value as the public GetDistance function. See GetDistance above for more information", EditorStyles.wordWrappedLabel );

		GUILayout.Space( paragraphSpace );

		// SimpleJoystick.GetJoystickState
		EditorGUILayout.LabelField( "bool SimpleJoystick.GetJoystickState( string joystickName )", itemHeaderStyle );
		EditorGUILayout.LabelField( "Returns the state that the joystick is currently in. This function will return true when the joystick is being interacted with, and false when not.", EditorStyles.wordWrappedLabel );
		
		EditorGUILayout.EndScrollView();
	}
	
	void Extras ()
	{
		overview.scrollPosition = EditorGUILayout.BeginScrollView( overview.scrollPosition, false, false, docSize );
		EditorGUILayout.LabelField( "Videos", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   The links below are to the collection of videos that we have made in connection with the Simple Joystick. The Tutorial Videos are designed to get the Simple Joystick implemented into your project as fast as possible, and give you a good understanding of what you can achieve using it in your projects, whereas the demonstrations are videos showing how we, and others in the Unity community, have used assets created by Tank and Healer Studio in our projects.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Tutorials", buttonSize ) )
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TnIHHL85cwLDIw5O85JMCh1" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Demonstrations", buttonSize ) )
			Application.OpenURL( "https://www.youtube.com/playlist?list=PL7crd9xMJ9TlkjepDAY_GnpA1CX-rFltz" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Example Scripts", sectionHeaderStyle );
		EditorGUILayout.LabelField( "   Below is a link to a list of free example script that we have made available on our support website. Please feel free to use these as an example of how to get started on your own scripts. The scripts provided are fully commented to help you to grasp the concept behind the code. These script are by no means a complete solution, and are not intended to be used in finished projects.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Example Scripts", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/sj-example-scripts.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
	
	void OtherProducts ()
	{
		overview.scrollPosition = EditorGUILayout.BeginScrollView( overview.scrollPosition, false, false, docSize );

		/* ------------ < ULTIMATE STATUS BAR > ------------ */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( sectionSpace );
		GUILayout.Label( usbPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( "Ultimate Status Bar", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   The Ultimate Status Bar is a complete solution for displaying your character's current health, mana, energy, stamina, experience, or virtually any condition that you'd like your player to be aware of. It can also be used to show a selected target's health, the progress of loading or casting, and even interacting with objects. Whatever type of progress display that you need, the Ultimate Status Bar can make it visually happen. Additionally, you have the option of using the many \"Ultimate\" textures provided, or you can easily use your own. If you are looking for a way to neatly display any type of status for your game, then consider the Ultimate Status Bar.", EditorStyles.wordWrappedLabel );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-status-bar.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* -------------- < END STATUS BAR > --------------- */

		GUILayout.Space( 25 );

		/* -------------- < ULTIMATE BUTTON > -------------- */
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Space( 15 );
		GUILayout.Label( ubPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( "Ultimate Button", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   Buttons are a core element of UI, and as such they should be easy to customize and implement. The Ultimate Button is the embodiment of that very idea. This code package takes the best of Unity's Input and UnityEvent methods and pairs it with exceptional customization to give you the most versatile button for your mobile project. Are you in need of a button for attacking, jumping, shooting, or all of the above? With Ultimate Button's easy size and placement options, style options, and touch actions, you'll have everything you need to create your custom buttons, whether they are simple or complex.", EditorStyles.wordWrappedLabel );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-button.html" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END ULTIMATE BUTTON > ------------ */

		EditorGUILayout.EndScrollView();
	}
	
	void Feedback ()
	{
		feedback.scrollPosition = EditorGUILayout.BeginScrollView( feedback.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "Having Problems?", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you experience any issues with the Simple Joystick, please contact us right away. We will lend any assistance that we can to resolve any issues that you have.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "<b>Support Email:</b>", paragraphStyle );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Good Experiences?", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you have appreciated how easy the Simple Joystick is to get into your project, leave us a comment and rating on the Unity Asset Store. We are very grateful for all positive feedback that we get.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Rate Us", buttonSize ) )
			Application.OpenURL( "https://www.assetstore.unity3d.com/#!/content/28685" );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Show Us What You've Done!", sectionHeaderStyle );

		EditorGUILayout.LabelField( "   If you have used any of the assets created by Tank & Healer Studio in your project, we would love to see what you have done. Contact us with any information on your game and we will be happy to support you in any way that we can!", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "<b>Contact Us:</b>", paragraphStyle );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );

		GUILayout.Space( 25 );

		EditorGUILayout.EndScrollView();
	}
	
	void ThankYou ()
	{
		thankYou.scrollPosition = EditorGUILayout.BeginScrollView( thankYou.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "    The two of us at Tank & Healer Studio would like to thank you for purchasing the Simple Joystick asset package from the Unity Asset Store. If you have any questions about the Simple Joystick that are not covered in this Documentation Window, please don't hesitate to contact right away.", EditorStyles.wordWrappedLabel );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "<b>Support Email:</b>", paragraphStyle );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "    We hope that the Simple Joystick will be a great help to you in the development of your game. After pressing the continue button below, you will be presented with helpful information on this asset to assist you in implementing Simple Joystick into your project.\n", EditorStyles.wordWrappedLabel );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", buttonSize ) )
			NavigateBack();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}
	
	void ChangeLog ()
	{
		changeLog.scrollPosition = EditorGUILayout.BeginScrollView( changeLog.scrollPosition, false, false, docSize );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Future Updates", buttonSize ) )
			NavigateForward( futureUpdates );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.3.0", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Many improvements to the Simple Joystick functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Many improvements to the Documentation Window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new Minimalist joystick texture.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed Third Person Example scene to avoid conflicts with Unity's default scripts.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new 'Asteroids Example' to help better show how the Simple Joystick can be used.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Renamed <i>Textures</i> folder to <i>Sprites</i> to help be more consistent throughout our packages.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Created new <i>Prefabs</i> folder to contain easy to use prefabs of the Simple Joystick.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Changed the creation menu from 'GameObject / UI / Ultimate UI / Simple Joystick' to 'GameObject / UI / Simple Joystick'.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Renamed the function JoystickPosition to GetPosition.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Renamed the function JoystickDistance to GetDistance.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Renamed the function JoystickState to GetJoystickState.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new function: DisableJoystick(). For more information about it's use, please see the Documentation section of this window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new function: EnableJoystick(). For more information about it's use, please see the Documentation section of this window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new function: GetHorizontalAxis(). For more information about it's use, please see the Documentation section of this window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new function: GetVerticalAxis(). For more information about it's use, please see the Documentation section of this window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added new function: GetSimpleJoystick(). For more information about it's use, please see the Documentation section of this window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the UpdatePositioning() static function.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the ResetJoystick static function.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.2.2", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Small update to the Documentation Window.", paragraphStyle );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.2.1", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Improved overall performance.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Fixed errors that happen with Unity's Canvas Scaler component in version 5.3.3+.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Integrated Updater script into the SimpleJoystick.cs script.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Integrated the Simple Joystick creator script into the SimpleJoystickEditor.cs script.", paragraphStyle );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.2.0", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Complete overhaul to the Simple Joystick's functionality.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added tons of new options, and improved older ones.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Added static referencing for ease of use.", paragraphStyle );
		
		EditorGUILayout.EndScrollView();
	}

	void FutureUpdates ()
	{
		changeLog.scrollPosition = EditorGUILayout.BeginScrollView( changeLog.scrollPosition, false, false, docSize );

		EditorGUILayout.LabelField( "  • There are no planned updates other than maintenance and fixing any bugs that come to our attention.", paragraphStyle );

		EditorGUILayout.EndScrollView();
	}

	void VersionChanges ()
	{
		changeLog.scrollPosition = EditorGUILayout.BeginScrollView( changeLog.scrollPosition, false, false, docSize );
		GUILayout.Space( itemHeaderSpace );
		
		EditorGUILayout.LabelField( "  Thank you for downloading the most recent version of the Simple Joystick. As always, if you run into any issues with the Simple Joystick, please contact us right away.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "<b>Support Email:</b>", paragraphStyle );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );

		GUILayout.Space( sectionSpace );
		
		EditorGUILayout.LabelField( "There a few changes in Simple Joystick version 1.3.0 that may affect your scripts if they were using some of the Simple Joystick's public functions. Below is a list of some of the changes to be aware of.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Renamed Functions", sectionHeaderStyle );
		EditorGUILayout.LabelField( "  • JoystickPosition is now named GetPosition to better represent it's purpose.", paragraphStyle );
		EditorGUILayout.LabelField( "  • JoystickDistance is now named GetDistance to better represent it's purpose.", paragraphStyle );
		EditorGUILayout.LabelField( "  • JoystickState is now named GetJoystickState to better represent it's purpose.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "To see the complete ChangeLog, please click on the version number of this window.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", paragraphStyle, GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", buttonSize ) )
			NavigateBack();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndScrollView();
	}

	[InitializeOnLoad]
	class SimpleJoystickInitialLoad
	{
		static SimpleJoystickInitialLoad ()
		{
			// If the user has a older version of USB that used the bool for startup...
			if( EditorPrefs.HasKey( "SimpleJoystickStartup" ) && !EditorPrefs.HasKey( "SimpleJoystickVersion" ) )
			{
				// Set the new pref to 0 so that the pref will exist and the version changes will be shown.
				EditorPrefs.SetInt( "SimpleJoystickVersion", 0 );
			}

			// If this is the first time that the user has downloaded the Ultimate Status Bar...
			if( !EditorPrefs.HasKey( "SimpleJoystickVersion" ) )
			{
				// Set the current menu to the thank you page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "SimpleJoystickVersion", importantChanges );
				
				EditorApplication.update += WaitForCompile;
			}
			else if( EditorPrefs.GetInt( "SimpleJoystickVersion" ) < importantChanges )
			{
				// Set the current menu to the version changes page.
				NavigateForward( versionChanges );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "SimpleJoystickVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;
			
			InitializeWindow();
		}
	}
}