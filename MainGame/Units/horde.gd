extends Node3D
class_name Horde

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
func addUnit(tier : int = 1, health = 1, damage = 1, speed = 1):
	# Load new unit
	var newUnit = null
	if tier == 1:
		newUnit = load("res://Units/Unit - Tier 1.tscn").instantiate()
	elif tier == 2:
		newUnit = load("res://Units/Unit - Tier 2.tscn").instantiate()
	elif tier == 3:
		newUnit = load("res://Units/Unit - Tier 3.tscn").instantiate()
	else:
		newUnit = load("res://Units/unit.tscn").instantiate()
	
	
	# Set new unit stats
	newUnit.health = health
	newUnit.damage = damage
	newUnit.speed = speed
	
	# Add unit as child of horde and element in units
	units.append(newUnit)
	add_child(newUnit)
	unitAdded.emit()
	
	# Reposition to accomodating changing horde size
	repositionUnits()
	
	# Recalculate average speed
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
	# Load new horde
	var newHorde = load("res://Units/horde.tscn").instantiate()
	
	# Move units to the new horde
	if numUnits <= units.size():
		for i in range(numUnits):
			newHorde.addUnit(units[0].tier, units[0].health, units[0].damage, units[0].speed)
			units[0].queue_free()
			units.remove_at(0)
	
	# Reposition units in current horde to account for now missing units
	repositionUnits()
	
	# Instantiate the new horde
	get_parent().add_child(newHorde)
	mitosisHappened.emit()
	return newHorde

# Commands the horde to move to a specific coordinate (x, z)
# No need to include y because the horde never moves up or down
func startMoving(xCoord : float, zCoord : float):
	look_at(Vector3(xCoord, position.y, zCoord))
	moving = true
	targetX = xCoord
	targetZ = zCoord
	startedMoving.emit()
	

# Called in _physics_process to move the horde at a constant speed to the target coordinates
func move(delta : float):
	# Calculate move
	var distanceX : float = targetX - position.x
	var distanceZ : float = targetZ - position.z
	var angle = atan2(distanceZ, distanceX)
	var newPositionX = move_toward(position.x, targetX, avgSpeed * delta * abs(cos(angle)))
	var newPositionZ = move_toward(position.z, targetZ, avgSpeed * delta * abs(sin(angle)))
	
	# Actually move by changing position
	position.x = newPositionX
	position.z = newPositionZ
	
	# Check to see if target is reached and moving is still neccesary
	if position.x == targetX and position.z == targetZ:
		stopedMoving.emit()
		moving = false
		#print("Target reached")

# Returns the number of units in the horde
func getSize():
	return units.size()


func _on_hitbox_area_entered(area: Area3D) -> void:
	if area.is_in_group("Enemy"):
		if is_in_group("Assimilated"):
			takeDamage(area)
	if area.is_in_group("Assimilated"):
		if is_in_group("Enemy"):
			takeDamage(area)

func takeDamage(area : Area3D):
	
