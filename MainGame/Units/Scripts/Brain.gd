extends Unit
class_name Brain

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	get_parent().connect("startedMoving", _on_moving)
	get_parent().connect("stopedMoving", _on_stopped)

signal gameOver()

func _on_moving():
	$lordofthebiomind.get_node("AnimationPlayer").play()

func _on_stopped():
	print("ALL DONE")
	$lordofthebiomind.get_node("AnimationPlayer").stop()

func _unit_death():
	gameOver.emit()
