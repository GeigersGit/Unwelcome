var Target : Transform;
var SpringObj : Transform;

var Drag : float;//Drag
var SpringForce : float; //Spring
var SpringRB : Rigidbody;

private var LocalDistance : Vector3;//Distance between the two points
private var LocalVelocity : Vector3;//Velocity converted to local space

function FixedUpdate () {
	//Calculate the distance between the two points
	LocalDistance = Target.InverseTransformDirection(Target.position - SpringObj.position);
	SpringRB.AddRelativeForce((LocalDistance.x)*SpringForce,(LocalDistance.y)*SpringForce,(LocalDistance.z)*SpringForce);//Apply Spring
	
	//Calculate the local velocity of the SpringObj point
	LocalVelocity = (SpringObj.InverseTransformDirection(SpringRB.velocity));
	SpringRB.AddRelativeForce(-LocalVelocity.x*Drag,-LocalVelocity.y*Drag,-LocalVelocity.z*Drag);//Apply Drag
}