extends Node3D
class_name Horde

# =======group vars========
var units = [] # Contains pointers to all of the unit nodes in the horde
var avgSpeed : float # The average of the speed stats of the horde's units
var totDamage : int

@export var radius : float = 2.0; # How far units spawn from the 1st unit

# =============================

# ======Movement vars==========
var moving:bool = false # Controls whether the horde is moving
var target:Vector3 = Vector3.ZERO # Target destination
var _enemy_Horde:Horde = null # Enemy Hoard that's being targeted

# =============================

# =======hoard state vars======
var sacrifice:bool = false # Once horde reaches destination they will die
enum horde_types {
	KING,
	ASSIMALTED_ACTIVE,
	ASSIMALTED_SPENT,
	ENEMY,
	IDLE
}
@export_enum("KING", "ASSIMALTED_ACTIVE", "ASSIMALTED_SPENT", "ENEMY") var horde_type:int = horde_types.ENEMY

enum states {
	IDLE,
	MOVING_TO,
	TARGETING,
	COMBAT,
}
var state:int = states.IDLE
# =============================

# =======combat vars===========
var harmable:bool = true # Turns off for immunity frames
var in_combat:bool = false
@onready var hitbox: Area3D = $Hitbox
# =============================

# =======Signals===============
signal startedMoving()
signal stoppedMoving()
signal unitAdded(horde:Horde, tier:int)
signal mitosisHappened()
signal deadHorde(horde:Horde)
signal sacrificed(tier:int, count:int)
signal gateDamaged(damage:int)

# ==========Starting functions=========================
## Called when the node enters the scene tree for the first time.
func _ready() -> void:
	# Make the hitbox shape unique to this horde
	$Hitbox/CollisionShape3D.shape = $Hitbox/CollisionShape3D.shape.duplicate()
	
	match horde_type:
		horde_types.KING:
			Globals.King = self
			addUnit(999)
			get_parent().addKing(self)
			connectToHoardManager()
			add_to_group("Assimilated")
		horde_types.ASSIMALTED_ACTIVE:
			connectToHoardManager()
			add_to_group("Assimilated")
		horde_types.ASSIMALTED_SPENT:
			pass
		_: # ENEMY
			# SET TARGET TO BE KING
			targetEnemy(Globals.King)
			add_to_group("Enemy")
			state = states.TARGETING
			pass

## If this is a friendly horde this will be called to connect to the manager
func connectToHoardManager()->void:
	unitAdded.connect(get_parent()._on_unit_added)
	deadHorde.connect(get_parent().handleDeadHorde)

# =======================================================

func change_state(new_state):
	#print(new_state)
	state = new_state

## Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
	match state:
		states.IDLE:
			pass
		states.MOVING_TO:
			move(delta)
		states.TARGETING:
			if !_enemy_Horde:
				change_state(states.IDLE)
				stoppedMoving.emit()
			else:
				target = _enemy_Horde.global_position
				move(delta)
			
		states.COMBAT:
			pass
		_:
			print("UNKNOWN STATE ", state)
	
	for area in hitbox.get_overlapping_areas():
		checkForDamage(area)


# =========Horde Setting Functions ===================
#region Horde Setting Functions
## Recalculates horde after unit change
func recalc():
	# Reposition to accomodating changing horde size
	repositionUnits()
	
	# Recalculate average speed and total damage
	setAvgSpeed()
	setTotDamage()
	setHitBoxSize()

## Helper method for repositionUnits
func setPosition(unit : Node, index : int):
	if index > 1:
		var rotationAngle = 2 * PI * index / (units.size() -1)
		unit.position.z = radius * cos(rotationAngle)
		unit.position.x = radius * sin(rotationAngle)
	else:
		unit.position = Vector3.ZERO

## Calculates and stores the average speed from the unit stats
func setAvgSpeed():
	var total = 0
	for unit in units:
		total += unit.speed
	avgSpeed = total / units.size()

## Sets the total damage from the unit stats
func setTotDamage():
	var total = 0
	for unit in units:
		total += unit.damage
	totDamage = total

## Positions the units in the horde to be evenly distributed around the 1st unit
func repositionUnits():
	var index = 1;
	for unit in units: 
		setPosition(unit, index)
		index += 1

## Sets the size of the hitbox to grow or shrink depending on howmany units are int it
func setHitBoxSize():
	$Hitbox/CollisionShape3D.shape.radius = pow(4 * units.size(), .33) + 2.0 

## Returns the number of units in the horde
func getSize()->int:
	if horde_type == horde_types.KING:
		return units.size() + 9
	return units.size()

#endregion
# ======================================================

# ========Horde Unit Changes Functions=================
#region Horde Unit Changes Functions
## Adds a unit to the horde. Use parameters to set it's stats
func addUnit(tier : int = 1):
	# Load new unit
	var newUnit = null
	match tier:
		999:
			newUnit = load("res://Units/Scenes/king.tscn").instantiate()
		2:
			newUnit = load("res://Units/Scenes/Unit - Tier 2.tscn").instantiate()
		3: 
			newUnit = load("res://Units/Scenes/Unit - Tier 3.tscn").instantiate()
		_:
			newUnit = load("res://Units/Scenes/Unit - Tier 1.tscn").instantiate()
	
	# Add unit as child of horde and element in units
	units.append(newUnit)
	add_child(newUnit)
	print("added ", tier)
	unitAdded.emit(self, tier)
	
	recalc()

## Kills a unit from the horde
func removeUnit()->void:
	var unit:Unit = units.pop_back()
	unit.die()


## Removes numUnits units from the horde and puts them in a new horde.
## Returns a reference to the new horde
func mitosis(numUnits : int) -> Horde:
	# Safty check
	if numUnits > units.size() or numUnits < 1:
		return null
	# Load new horde
	var newHorde:Horde = load("res://Units/Scenes/horde.tscn").instantiate()
	newHorde.horde_type = horde_types.ASSIMALTED_ACTIVE
	newHorde.global_position = global_position + Vector3(radius * 2.5, 0, 0)
	get_parent().add_child(newHorde)
	
	#print(numUnits)
	# Move units to the new horde
	for i in range(numUnits):
		var unit:Unit = units.pop_back()
		remove_child(unit)
		newHorde.add_child(unit)
		newHorde.units.append(unit)
	
	# Recalc units in current horde to account for now missing units
	recalc()
	
	# Instantiate the new horde
	mitosisHappened.emit()
	# Ensure the new horde has correct stats and is positioned nearby
	newHorde.recalc()
	
	return newHorde
#endregion
# ====================================================

# ============Horde Movement Funcions==================
#region Horde Movement Functions
# Commands the horde to move to a specific coordinate (x, z)
# No need to include y because the horde never moves up or down
func startMoving(dest:Vector3):
	look_at(Vector3(dest.x, position.y, dest.z))
	moving = true
	target.x = dest.x
	target.z = dest.z
	startedMoving.emit()

func targetEnemy(enemy:Horde)->void:
	if state != states.TARGETING:
		change_state(states.TARGETING)
	_enemy_Horde = enemy
	startMoving(enemy.global_position)

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
		target = Vector3.ZERO
		moving = false
		if sacrifice:
			sacrificeSelf()
		#print("Target reached")
#endregion

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
	


func spendHorde(dest:Vector3)->void:
	$Hitbox.set_deferred("monitorable", false)
	$Hitbox.set_deferred("monitoring", false)
	startMoving(dest)
	sacrifice = true


# =============Combat Functions==============================
#region Combat Functions
## Runs in physics_process for each area
func checkForDamage(area: Area3D):
	var horde = area.get_parent()
	
	# Case #1 where damage should be dealt
	if horde.is_in_group("Enemy") and is_in_group("Assimilated"):
		#print("Assimilated horde takes damage")
		in_combat = true
		if state != states.COMBAT:
			change_state(states.COMBAT)
			stoppedMoving.emit()
		if harmable:
			takeDamage(horde)
			harmable = false
			$ImmunityFrames.start()
	# Case #2 where damage should be dealt
	elif horde.is_in_group("Assimilated") and is_in_group("Enemy"):
		#print("Enemy horde takes damage")
		in_combat = true
		if state != states.COMBAT:
			change_state(states.COMBAT)
			stoppedMoving.emit()
			
		if harmable:
			takeDamage(horde)
			harmable = false
			$ImmunityFrames.start()
	# Case #3 where gate needs to take damage from this horde
	elif horde.is_in_group("Gate") and is_in_group("Enemy") and !horde.broken:
		in_combat = true
		if state != states.COMBAT:
			change_state(states.COMBAT)
			stoppedMoving.emit()
			
		gateDamaged.emit(totDamage)
	elif in_combat:
		in_combat = false
		if _enemy_Horde:
			change_state(states.TARGETING)
		elif target != Vector3.ZERO:
			print("No Target")
			change_state(states.MOVING_TO)



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
#endregion
# ==========================================================

# =============Signal Handelers============================
func _on_immunity_frames_timeout() -> void:
	harmable = true
	#print("Immunity frames over")


func _on_hitbox_input_event(camera: Node, event: InputEvent, event_position: Vector3, normal: Vector3, shape_idx: int) -> void:
	if !is_in_group("Enemy"):
		return
	if event is InputEventMouseButton:
		if event.pressed and  event.button_index == MOUSE_BUTTON_LEFT:
			print("Clicked!")
			$HordeActionHandeler.Attack(get_parent().get_node("HordeManager").getSelectedHorde(), self)
			print(get_parent().get_node("HordeManager").SelectedHorde)
			
	#if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT and event.pressed \
		#and horde_type == horde_types.ENEMY:
		#$HordeActionHandeler.Attack(get_parent().getSelectedHorde(), self)
