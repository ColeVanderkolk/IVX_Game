extends Node3D

@onready var horde: Node3D = $Horde

var testLocations = [Vector2(1, 2), 
					Vector2(-2, 4), 
					Vector2(3, 4),
					Vector2(3, 10),
					Vector2(-10, -10),
					Vector2(0,0)]
var i = 0
var s = 1

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	horde.addUnit()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	
	if !horde.moving and testLocations.size() > i:
		var x = testLocations[i].x
		var y = testLocations[i].y
		horde.startMoving(x, y)
		i += 1


func _on_timer_timeout() -> void:
	if s < 10:
		$Timer.start()
		horde.addUnit(1, 1, i * 5)
		s = horde.units.size()
	
	if i == testLocations.size():
		i = 0
		horde = horde.mitosis(2)
