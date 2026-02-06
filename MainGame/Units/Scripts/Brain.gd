extends Unit
class_name Brain

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	get_parent().connect("startedMoving", _on_moving)
	get_parent().connect("stopedMoving", _on_stopped)

signal gameOver()

func _on_moving():
	$lordofthebiomind.AnimationPlayer.Play()

func _on_stopped():
	$lordofthebiomind.AnimationPlayer.Stop()

func _unit_death():
	gameOver.emit()
