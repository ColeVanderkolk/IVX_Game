extends CharacterBody3D
class_name Unit

@export var health : int
@export var damage : int
@export var speed : float
@export var tier : int

# Signals
signal unitDeath()
signal unitTakesDamage()

func _physics_process(_delta: float) -> void:
	pass

func takeDamage(damageTaken : int):
	unitTakesDamage.emit()
	health -= damageTaken
	if health < 0:
		die()
		return -health

func die():
	unitDeath.emit()
	queue_free()
