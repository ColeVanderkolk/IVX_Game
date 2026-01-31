extends Node3D

var units = []
var avgSpeed : float

@export var radius : float = 2.0;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass

func addUnit():
	var newUnit = load("res://Units/unit.tscn").instantiate()
	units.append(newUnit)
	add_child(newUnit)
	
	repositionUnits()
	
	setAvgSpeed()

func setPosition(unit : Node, index : int):
	if index > 1:
		var rotationAngle = 2 * PI * index / (units.size() -1)
		unit.position.z = 2 * cos(rotationAngle)
		unit.position.x = 2 * sin(rotationAngle)
	else:
		unit.position = Vector3.ZERO

func setAvgSpeed():
	var total = 0
	for unit in units:
		total += unit.speed
	avgSpeed = total / units.size()

func repositionUnits():
	var index = 1;
	for unit in units: 
		setPosition(unit, index)
		index += 1

func mitosis(numUnits : int):
	var newHorde = load("res://Units/horde.tscn").instantiate()
	
	for i in range(numUnits):
		units[0].queue_free()
		units.remove_at(0)
		newHorde.addUnit()
	
	repositionUnits()
	
	get_parent().add_child(newHorde)
	return newHorde
