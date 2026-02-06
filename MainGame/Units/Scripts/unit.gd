extends CharacterBody3D
class_name Unit

@export var health : int
@export var damage : int
@export var speed : float
@export var tier : int

# Signals
signal unitDeath()

@onready var death_timer:Timer = Timer.new()

func _ready() -> void:
	add_child(death_timer)
	death_timer.timeout.connect(self._on_death)

func _physics_process(_delta: float) -> void:
	pass

# Currently just removes the unit from the game, but should be modified to have different effects on deathh
func die():
	$fleshcreature1plus/AnimationPlayer.play("Armature_002")
	unitDeath.emit()
	death_timer.start(4)

func _on_death():
	queue_free()
