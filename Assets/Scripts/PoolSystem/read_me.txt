Matteo Beltrame
bltmtt@gmail.com

Pool System

System organization
PoolManager: attach this component to a gameObject inside the scene. From here is possible to use directly the editor to add pool categories and pools to those
categories. It is recommended to use only the PoolManager and to not access PoolCategory methods.

IMPORTANT:
Remember to implement the interface IPooledObject in every class that is indeed pooled. In the object pooling paradigm objects are not destroyed but instead deactivated
so the Unity method Start will be called only one time, at the beginning of the level. Use instead the IPooledObject method OnObjectSpawn() for initialization that you want
to happen at every object "spawn", this method is called by the PoolManager on every object and subObjects when the object is pooled into the level.

Utility example:
The categories represent a method to classify pools. Only one PoolManager is indeed needed. For example it is possible to add categories like "Bullets", "Obstacles"
and so on. Inside these categories it is possible to create Pools, for example in categories "Bullets" there could be pools like "CannonBullets" or "PistolBullets",
and calling the method Spawn passing both tags, category and pool, will spawn an object of that pool without considering the spawn probability of the pool. 
For the category "Obstacles" instead, it could be useful that every time the obstacle needs to be spawned, a random pool is picked within the category based
on the spawn probability of the pool and then one of the pooled object of that pool is spawned and returned, in this case the method to use is the method Spawn but
passing only the category tag.

Tip: Double check that the pool dimension is correctly set in the editor and its at least 1

Methods
--------------------------------------------------------------------------------------------------------------------------------------------------------------------
- PoolManager GetInstance()
	Return the reference of the PoolManager in the scene. Note THERE CAN BE ONLY ONE POOLMANAGER INSIDE A SCENE (why would someone need more than one anyway)

- GameObject Spawn(string categoryName, string poolTag, Vector3 position, Quaternion rotation)
	Spawn, or better, Pool an object into the scene specifying pool tag and category tag.

- GameObject Spawn(string categoryName, Vector3 position, Quaternion rotation)
	Pool an object into the scene specifying only the category tag, the object will be automatically picked randomly from all the pools inside the category based on 
	each pool probability.

- void DeactivateObject(GameObject objectToDeactivate)
	When using pools, objects have to be deactivated instead of destroyed, use this function instead of calling Destroy(gameObject).
	The object will be deactivated and will just be ready to be spawned again for future uses, saving performance.