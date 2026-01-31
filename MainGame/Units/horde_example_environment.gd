extends Node3D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	#$Horde.position.x -= 5
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	$Horde.addUnit()
	#var new = $Horde.mitosis(2)
	#new.position. x += 5


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass
