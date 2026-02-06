extends Node3D
class_name Horde

var units = [] # Contains pointers to all of the unit nodes in the horde
var avgSpeed : float # The average of the speed stats of the horde's units
var totDamage : int

@export var radius : float = 2.0; # How far units spawn from the 1st unit

var moving:bool = false # Controls whether the horde is moving
var target:Vector3 = Vector3.ZERO

var _enemy_Horde:Horde = null

var sacrifice:bool = false # Once horde reaches destination they will die

var harmable:bool = true # Turns off for immunity frames
var in_combat:bool = false

# Signals
signal startedMoving()
signal stoppedMoving()
signal unitAdded(horde:Horde, tier:int)
signal mitosisHappened()
signal deadHorde(horde:Horde)
signal sacrificed(tier:int, count:int)
signal gateDamaged(damage:int)

@onready var hitbox: Area3D = $Hitbox

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Make the hitbox shape unique to this horde
	$Hitbox/CollisionShape3D.shape = $Hitbox/CollisionShape3D.shape.duplicate()

func connectToHoardManager()->void:
	unitAdded.connect(get_parent()._on_unit_added)
	deadHorde.connect(get_parent().handleDeadHorde)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
	if moving and !in_combat:
		move(delta)
	
	if in_combat:
		in_combat = false
	for area in hitbox.get_overlapping_areas():
		checkForDamage(area)

# Adds a unit to the horde. Use parameters to set it's stats
func addUnit(tier : int = 1):
	# Load new unit
	var newUnit = null
	match tier:
		2:
			newUnit = load("res://Units/Scenes/Unit - Tier 2.tscn").instantiate()
		3: 
			newUnit = load("res://Units/Scenes/Unit - Tier 3.tscn").instantiate()
		_:
			newUnit = load("res://Units/Scenes/Unit - Tier 1.tscn").instantiate()
	
	# Add unit as child of horde and element in units
	units.append(newUnit)
	add_child(newUnit)
	unitAdded.emit(self, tier)
	
	recalc()

func removeUnit()->void:
	var unit:Unit = units.pop_back()
	unit.die()

# Recalculates horde after unit change
func recalc():
	# Reposition to accomodating changing horde size
	repositionUnits()
	
	# Recalculate average speed and total damage
	setAvgSpeed()
	setTotDamage()
	setHitBoxSize()

# Helper method for repositionUnits
func setPosition(unit : Node, index : int):
	if index > 1:
		var rotationAngle = 2 * PI * index / (units.size() -1)
		unit.position.z = radius * cos(rotationAngle)
		unit.position.x = radius * sin(rotationAngle)
	else:
		unit.position = Vector3.ZERO

# Calculates and stores the average speed from the unit stats
func setAvgSpeed():
	var total = 0
	for unit in units:
		total += unit.speed
	avgSpeed = total / units.size()

# Sets the total damage from the unit stats
func setTotDamage():
	var total = 0
	for unit in units:
		total += unit.damage
	totDamage = total

# Positions the units in the horde to be evenly distributed around the 1st unit
func repositionUnits():
	var index = 1;
	for unit in units: 
		setPosition(unit, index)
		index += 1

func setHitBoxSize():
	$Hitbox/CollisionShape3D.shape.radius = pow(4 * units.size(), .33) + 2.0 

# Removes numUnits units from the horde and puts them in a new horde.
# Returns a reference to the new horde
func mitosis(numUnits : int) -> Horde:
	# Safty check
	if numUnits > units.size() or numUnits < 1:
		return null
	
	# Load new horde
	var newHorde:Horde = load("res://Units/Scenes/horde.tscn").instantiate()
	
	# Move units to the new horde
	for i in range(numUnits):
		var unit:Unit = units.pop_back()
		newHorde.add_child(unit)
	
	# Recalc units in current horde to account for now missing units
	recalc()
	
	# Instantiate the new horde
	get_parent().add_child(newHorde)
	mitosisHappened.emit()
	return newHorde

# Commands the horde to move to a specific coordinate (x, z)
# No need to include y because the horde never moves up or down
func startMoving(dest:Vector3):
	look_at(Vector3(dest.x, position.y, dest.z))
	moving = true
	target.x = dest.x
	target.z = dest.z
	startedMoving.emit()

func targetEnemy(enemy:Horde)->void:
	_enemy_Horde = enemy
	startMoving(enemy.global_position)

func spendHorde(dest:Vector3)->void:
	#$Hitbox.set_deferred("monitorable", false) #these throw errors in physics loop
	#$Hitbox.set_deferred("monitoring", false)
	startMoving(dest)
	sacrifice = true

# Called in _physics_process to move the horde at a constant speed to the target coordinates
func move(delta : float):
	# Calculate move
	var distanceX : float = target.x - position.x
	var distanceZ : float = target.z - position.z
	var angle = atan2(distanceZ, distanceX)
	var newPositionX = move_toward(position.x, target.x, avgSpeed * delta * abs(cos(angle)))
	var newPositionZ = move_toward(position.z, target.z, avgSpeed * delta * abs(sin(angle)))
	
	# Actually move by changing position
	position.x = newPositionX
	position.z = newPositionZ
	
	# Check to see if target is reached and moving is still neccesary
	if position.x == target.x and position.z == target.z:
		stoppedMoving.emit()
		moving = false
		if sacrifice:
			sacrificeSelf()
		#print("Target reached")

# Kill all units in this horde (to assimillate or upgrade)
# in the event of an upgrade, all units presumed same tier
func sacrificeSelf()->void:
	var sacUnit:Unit = units.get(0)
	var sacTier = sacUnit.tier
	var sacCount = units.size()
	while !units.is_empty():
		removeUnit()
	sacrificed.emit(sacTier, sacCount)
	deadHorde.emit() #just in case manager was tracking this
	queue_free()
	
# Returns the number of units in the horde
func getSize()->int:
	return units.size()

# Runs in physics_process for each area
func checkForDamage(area: Area3D):
	var horde = area.get_parent()
	
	# Case #1 where damage should be dealt
	if horde.is_in_group("Enemy") and is_in_group("Assimilated"):
		#print("Assimilated horde takes damage")
		in_combat = true
		if harmable:
			takeDamage(horde)
			harmable = false
			$ImmunityFrames.start()
	# Case #2 where damage should be dealt
	elif horde.is_in_group("Assimilated") and is_in_group("Enemy"):
		#print("Enemy horde takes damage")
		in_combat = true
		if harmable:
			takeDamage(horde)
			harmable = false
			$ImmunityFrames.start()
	# Case #3 where gate needs to take damage from this horde
	elif horde.is_in_group("Gate") and is_in_group("Enemy"):
		in_combat = true
		gateDamaged.emit(totDamage)


func takeDamage(attacker : Horde):
	var damageTaken = attacker.totDamage
	
	# Apply damage to units
	for unit:Unit in units:
		damageTaken -= unit.health
		if damageTaken < 0:
			unit.health = abs(damageTaken)
			break
		else:
			var deadUnit:Unit = unit
			
			# Assimilates enemy
			if attacker.is_in_group("Assimilated"):
				attacker.addUnit(deadUnit.tier)
			
			var _dead_glob_pos = deadUnit.global_position
			remove_child(deadUnit)
			get_parent().add_child(deadUnit)
			deadUnit.global_position = _dead_glob_pos
			units.erase(unit)
			deadUnit.die()
			
			# Remove the horde from the game if it's empty
			if units.size() < 1:
				deadHorde.emit(self)
				queue_free()
			else:
				recalc()


func _on_immunity_frames_timeout() -> void:
	harmable = true
	#print("Immunity frames over")
