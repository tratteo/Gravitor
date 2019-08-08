Matteo Beltrame
bltmtt@gmail.com

Spawner System

1. Create a Spawner passing the context (MonoBehaviour) and your spawn area range
2. Create a timer within the spawner calling CreateSpawnTimer
3. Subscribe delegate method to spawn event calling SubscribeToSpawnTimerEvent
4. Create when needed a timed spawn exception calling CreateSpawnException and either start it on creation or later calling StartTimedException
5. Get the random spawn position in the area of the Spawner calling GetSpawnPosition, the Spawner will take care of an eventual spawn area exception


Constructor:
- Spawner(MonoBehaviour context, Vector2 horizontalRange, Vector2 verticalRange, Vector2 depthRange)
	The context represents the MonoBehaviour your creating this instance from, the 3 ranges represents the 3D area you want the object to spawn in


In all methods the startNow bool indicates if the timer should start at the moment of the creation.

Spawn timer methods
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
- SpawnTimer CreateSpawnTimer(int fixedSpawnRate, bool startNow)
	SpawnEvent will fire X times per second, with X = fixedSpawnRate


- SpawnTimer CreateSpawnTimer(Vector2 spawnRateRange, bool startNow)
	SpawnEvent will fire X times per second, with spawnRateRange.x < X < spawnRateRange.y, at every event fire the spawnRate will update to a
	random value inside the range.


- SpawnTimer CreateSpawnTimer(Func<int, float> scaleOverTimeFunc, bool startNow)
	SpawnEvent calls will be scaled in time with regards to a scaleOverTimeFunc. Note that the mathematical function passed as a parameter has to have 
	the following generic declaration: float <FuncName>(int param)
	Over the time (Time.timeSinceLevelLoad) the function will adjust the spawn rate. Useful to create functions that take care of incrementing 
	the difficulty over time.


- void StartSpawnTimer()
	Start the spawn timer (if not started at the moment of creation)


- void KillSpawnTimer()
	Kill the spawn timer if active


- void SubscribeToSpawnTimerEvent(Action functionToSub)
	Used to subscribe a method to the SpawnEvent, the method will be called everytime the SpawnEvent is fired. Note that the function needs 
	to have the following generic declaration: void <FuncName>(). Note that is possible to subscribe multiple functions one at a time, every time
	the SpawnEvent will occur, all the functions subscribed will be called.


- void public void UnsubscribeToSpawnEvent(Action functionToUnsub)
	Used to unsubscribe a method from the SpawnEven, meaning that the unsubscribed function will not be called when the SpawnEvent will occur.


Spawn exception methods
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
- SpawnException(MonoBehaviour context, Vector3 centre, float width, float height, float depth, float duration, bool startNow)
	Create a 3D rectangle spawn exception inside the area of spawn. The widh, height and depth represents the units from the centre of the cube in that axis,
	this means that geometrically speaking the rectangle edges length will be: width * 2, height * 2, depth * 2.
	To create a cube exception, for example, at position (x, y, z) with edge of N units, pass the position as the centre, and then N / 2 for all the dimensions.
	Note that if one bound of the exception exceeds the Spawner spawn area the bound of the exception will be set as the bound of the area.
	If the exception is completely outside the area a System.Exception will be thrown.


- SpawnException(MonoBehaviour context, Spawner spawner, Vector3 centre, float width, float height, float depth)
	Equal as before but in this case the exception is permanent. To stop the exception, call StopException.


- void StartTimedException()
	Start the exception created. It will persists until the end of the duration assigned at the creation.


- void StopException()
	Stop the current active exception.


- bool IsExceptionActive()
	If the exception is currently active.


- Vector3 GetSpawnPosition()
	Calling this function will provide a Vector3 representing a valid random position inside the 3D area of the Spawner considering an eventual SpawnException,
	this means that the function will not return a point inside the SpawnException if active.

	