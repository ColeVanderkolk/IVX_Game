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
	
	get_parent().connect("startedMoving", _on_move)
	get_parent().connect("stopedMoving", _on_stop)
	get_parent().connect("combat", _on_combat)
	get_parent().connect("combatEnds", _on_combat_end)
	
	add_child(audio)
	audio.max_distance = 25000
	audio.stream = load(death_sounds[randi_range(0, death_sounds.size()-1)])


func _on_move():
	$fleshcreature1plus/AnimationPlayer.play("Armature_003")

func _on_stop():
	$fleshcreature1plus/AnimationPlayer.stop()

func _on_combat():
	$fleshcreature1plus.visible = false
	$fleshcreature1plusattack.visible = true
	$fleshcreature1plusattack/AnimationPlayer.play("Armature_008")

func _on_combat_end():
	$fleshcreature1plus.visible = true
	$fleshcreature1plusattack.visible = false
	$fleshcreature1plusattack/AnimationPlayer.stop()

# Currently just removes the unit from the game, but should be modified to have different effects on deathh
func die():
	$fleshcreature1plus.visible = true
	$fleshcreature1plusattack.visible = false
	$fleshcreature1plusattack/AnimationPlayer.stop()
	$fleshcreature1plus/AnimationPlayer.play("Armature_002")
	audio.play()
	unitDeath.emit()
	death_timer.start(4)

func _on_death():
	queue_free()
