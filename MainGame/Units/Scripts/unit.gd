extends CharacterBody3D
class_name Unit

@export var health : int
@export var damage : int
@export var speed : float
@export var tier : int

# Signals
signal unitDeath()

@onready var death_timer:Timer = Timer.new()
@onready var audio:AudioStreamPlayer3D = AudioStreamPlayer3D.new()

var death_sounds = [
	"res://Assets/Audio/SFX/sfx_unitDeath_001.mp3",
	"res://Assets/Audio/SFX/sfx_unitDeath_002.mp3",
	"res://Assets/Audio/SFX/sfx_unitDeath_003.mp3"
]

func _ready() -> void:
	add_child(death_timer)
	death_timer.timeout.connect(self._on_death)
	
	add_child(audio)
	audio.stream = load(death_sounds[randi_range(0, death_sounds.size()-1)])

func _physics_process(_delta: float) -> void:
	pass

# Currently just removes the unit from the game, but should be modified to have different effects on deathh
func die():
	$fleshcreature1plus/AnimationPlayer.play("Armature_002")
	audio.play()
	unitDeath.emit()
	death_timer.start(4)

func _on_death():
	queue_free()
