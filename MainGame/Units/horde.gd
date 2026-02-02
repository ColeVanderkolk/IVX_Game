extends Node3D

var units = [] # Contains pointers to all of the unit nodes in the horde
var avgSpeed : float # The average of the speed stats of the horde's units

@export var radius : float = 2.0; # How far units spawn from the 1st unit

var moving = false # Controls whether the horde is moving
var targetX = null # The x value of where the horde is moving to
var targetZ = null # The z value of where the horde is moving to

# Signals
signal startedMoving()
signal stopedMoving()
signal unitAdded()
signal mitosisHappened()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
	if moving:
		move(delta)

# Adds a unit to the horde. Use parameters to set it's stats
func addUnit(health = 1, damage = 1, speed = 1, _tier : int = 1):
	var newUnit = load("res://Units/unit.tscn").instantiate()
	newUnit.health = health
	newUnit.damage = damage
	newUnit.speed = speed
	units.append(newUnit)
	add_child(newUnit)
	unitAdded.emit()
	
	repositionUnits()
	
	setAvgSpeed()

# Helper method for repositionUnits
func setPosition(unit : Node, index : int):
	if index > 1:
		var rotationAngle = 2 * PI * index / (units.size() -1)
		unit.position.z = 2 * cos(rotationAngle)
		unit.position.x = 2 * sin(rotationAngle)
	else:
		unit.position = Vector3.ZERO

# Calculates and stores the average speed from the unit stats
func setAvgSpeed():
	var total = 0
	for unit in units:
		total += unit.speed
	avgSpeed = total / units.size()

# Positions the units in the horde to be evenly distributed around the 1st unit
func repositionUnits():
	var index = 1;
	for unit in units: 
		setPosition(unit, index)
		index += 1

# Removes numUnits units from the horde and puts them in a new horde.
# Returns a reference to the new horde
func mitosis(numUnits : int):
	var newHorde = load("res://Units/horde.tscn").instantiate()
	
	for i in range(numUnits):
		units[0].queue_free()
		units.remove_at(0)
		newHorde.addUnit()
	
	repositionUnits()
	
	get_parent().add_child(newHorde)
	mitosisHappened.emit()
	return newHorde

# Commands the horde to move to a specific coordinate (x, z)
# No need to include y because the horde never moves up or down
func startMoving(xCoord : float, zCoord : float):
	#look_at(Vector3(xCoord, position.y, zCoord))
	moving = true
	targetX = xCoord
	targetZ = zCoord
	startedMoving.emit()
	

# Called in _physics_process to move the horde at a constant speed to the target coordinates
func move(delta : float):
	#var dirX = getDirection(position.x, targetX)
	#var dirZ = getDirection(position.z, targetZ)
	var distanceX : float = targetX - position.x
	var distanceZ : float = targetZ - position.z
	var angle = atan2(distanceZ, distanceX)#atan(distanceZ / distanceX)
	#print("current angle: " + str(angle))
	var newPositionX = move_toward(position.x, targetX, avgSpeed * delta * abs(cos(angle)))
	var newPositionZ = move_toward(position.z, targetZ, avgSpeed * delta * abs(sin(angle)))
	position.x = newPositionX
	position.z = newPositionZ
	#print("current x: " + str(newPositionX))
	#print("target x: " + str(targetX))
	#print("current z: " + str(newPositionZ))
	#print("target z: " + str(targetZ))
	if position.x == targetX and position.z == targetZ:
		stopedMoving.emit()
		moving = false
		#print("Target reached")
	
	#if dirX != getDirection(position.x, targetX):
	#	stopedMoving.emit()
	#	moving = false
	#if dirZ != getDirection(position.z, targetZ):
	#	stopedMoving.emit()
	#	moving = false

# Returns -1, 0, or 1 depending on what is needed for the horde to move in the right direction
# Helper method for move method
func getDirection(coord1, coord2):
	if coord1 > coord2:
		return -1
	elif coord1 == coord2:
		return 0
	else:
		return 1
