extends CharacterBody3D
class_name Unit

@export var health : int
@export var damage : int
@export var speed : float
@export var tier : int

# Signals
signal unitDeath()

func _physics_process(_delta: float) -> void:
	pass

# Currently just removes the unit from the game, but should be modified to have different effects on deathh
func die():
	unitDeath.emit()
	queue_free()
