Thank you for purchasing the Simple Joystick UnityPackage!

/* -----> IMPORTANT INFORMATION <----- */
Within Unity, please go to Window / Tank and Healer Studio / Simple Joystick to access important information on how to get started
using the Simple Joystick. In that window are sections like: How To, Overview, Documentation, Extras and more! Please
check out that window for the best and fullest experience.
/* ---> END IMPORTANT INFORMATION <--- */


/* IF YOU CAN'T VIEW THE SIMPLE JOYTICK WINDOW, READ THIS SECTION */
The Simple Joystick is organized into different folders so that everything can run smoothly, so first thing to do, is
understand the folders in connection with the Simple Joystick. There are three main folder that were imported with the
Simple Joystick: Editor, Editor Default Resources, and Plugins. The Editor Default Resources folder contains all of the
images, prefabs, and other things that help the Simple Joystick be as EASY as possible to work with inside the Editor.
It is worth noting that the files inside of this folder will NOT be included inside of any builds of your game. They are
strictly for use within the Editor. The Plugins folder contains all of the needed scripts and textures for the Simple
Joystick, and of course the Editor folder helps the Simple Joystick inspector be easier to navigate and use.

/* HOW TO REFERENCE THE SIMPLE JOYSTICK */
One of the great things about the Simple Joystick is the easy reference from other scripts. The first thing that
you will want to make sure to do is name the joystick in the Script Reference section. After that, we can reference
that particular Joystick by it's name. After the joystick has been named, we can get that joystick's position by
creating a Vector2 variable at runtime by calling a static function. This Vector2 will be the Simple Joystick's
position. Keep in mind that the Simple Joystick's LEFT AND RIGHT movement are translated into this Vector2's X
value, while the UP AND DOWN movement are the Vector2's Y value. Keep that in mind when applying the Simple
Joystick's position to characters.

For this example, let's assume that the joystick's name is Movement. In order to get that joystick's position, call
the static GetPosition function, and pass the desired joystick's name.

/* ------ < EXAMPLE > ------ */
EXAMPLE C#:
	Vector2 joystickPosition = SimpleJoystick.GetPosition( "Movement" );
EXAMPLE Java:
	var joystickPosition : Vector2 = SimpleJoystick.GetPosition( "Movement" );

After this, the joystickPosition variable can be used in anything that needs joystick input. For example, if you are
wanting to put the joystick's position into a character's movement script, you would create a Vector3 variable for
movement direction, and put in the appropriate values of the Simple Joystick's position.

/* ------ < EXAMPLE > ------ */
EXAMPLE C#:
	Vector3 movementDirection = new Vector3( joystickPosition.x, 0, joystickPosition.y );
EXAMPLE Java:
	var movementDirection : Vector3 = new Vector3( joystickPosition.x, 0, joystickPosition.y );

In the above example, the joystickPosition variable is used to give the movement direction values in the X and Z spots.
This is because you generally don't want your character to move in the Y direction unless the user jumps. That is why
we put the joystickPosition's Y value into the Z value of the movementDirection variable.

Understanding how to use the values from any input is important when creating character controllers, so experiment with
the values to try and understand how the mobile input can be used for different ways.