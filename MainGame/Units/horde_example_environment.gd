extends Node3D

var testLocations = [Vector2(1, 2), 
					Vector2(-2, 4), 
					Vector2(3, 4),
					Vector2(3, 10),
					Vector2(-10, -10)]
var i = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$Horde.addUnit()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	
	if !$Horde.moving and testLocations.size() > i:
		var x = testLocations[i].x
		var y = testLocations[i].y
		$Horde.startMoving(x, y)
		print(i)
		i += 1


func _on_timer_timeout() -> void:
	if $Horde.units.size() < 10:
		$Timer.start()
		$Horde.addUnit(1, 1, i)
