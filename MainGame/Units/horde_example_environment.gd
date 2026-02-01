extends Node3D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Horde.addUnit(1, 1, 1)
	
	$Horde.startMoving(200, 200)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass


func _on_timer_timeout() -> void:
	$Timer.start()
	$Horde.addUnit()
