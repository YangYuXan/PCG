# PCG

How to use:

1.First  create a empty object,and add a navMesh surface component.
![image](https://github.com/YangYuXan/PCG/assets/58082585/64175163-071b-4009-bc3a-d1ecdec178f4)



2.Put Dungeon prefab to level

![image](https://github.com/YangYuXan/PCG/assets/58082585/464b4663-ecf9-4872-906c-4974c7169fcb)




height and width is the map size

smooth set 2 or 3 is the best

White mark is the ground

G1-G4 is the wall


# Agent

There are two types of Agents,
The thief will randomly wander around the map when the Award Box is not detected. Once it sees contact (ray detection), it will run back to the starting point with the box.

![image](https://github.com/YangYuXan/PCG/assets/58082585/4a678c4b-39d0-4a2c-8c2c-7bed49476a91)





The guard (Troll) will patrol around the box, and once it finds the thief, it will chase him.

![image](https://github.com/YangYuXan/PCG/assets/58082585/4c3d8f80-bf1a-4ebb-aaf7-7d2347ba343b)






# Video
https://youtu.be/CXiLQKv9ZY4
