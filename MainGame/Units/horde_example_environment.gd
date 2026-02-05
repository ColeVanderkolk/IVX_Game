extends Node3D

# This script is only for me testing my horde.gd and unit.gd scripts.
# Not intended to be run for the actual games.

@onready var horde: Horde = $AssimilatedHorde
@onready var enemy_horde: Horde = $EnemyHorde

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
	horde.addUnit(3)
	horde.addUnit(1)
	horde.addUnit(2)
	enemy_horde.addUnit(3)
	enemy_horde.addUnit(3)
	enemy_horde.addUnit(3)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	
	if !horde.moving and testLocations.size() > i:
		var x = testLocations[i].x
		var y = testLocations[i].y
		horde.startMoving(x, y)
		i += 1
	elif !horde.moving and i == testLocations.size():
		horde = horde.mitosis(horde.getSize() / 2)
		horde.add_to_group("Assimilated")
		i = 0

func _on_timer_timeout() -> void:
	if s < 10:
		$Timer.start()
		horde.addUnit(3)
		s = horde.getSize()
