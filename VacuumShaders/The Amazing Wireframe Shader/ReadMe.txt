The Amazing Wireframe shader
By Davit Naskidashvili

VacuumShaders 2014
************************************


- Wireframe shader requires mesh with precomputed data stored inside color and vertex buffers.

- There are two ways to generate wireframe mesh:
1. Generate asset in editor. Select mesh and go to Assets/Component/VacuumShaders/The Amazing Wireframe Shader/Store wire inside Color/Generate and replace. 
   Asset file will be created and saved inside "Assets/Wireframed Meshes" folder.
2. Rebuild mesh at runtime. Assign TheAmazingWireframeGenerator script to the object Menu/Component/VacuumShaders/The Amazing Wireframe Generator


- Wireframe script generates only one instance of the mesh and saves it's ID to avoid same calulations in case of several request of the same mesh.
- Avoid using wireframed mesh on objects with multiple submaterials.


-Shader Optimizer (Assets/VacuumShaders/The Amazing Wireframe shader/Optimize shader) generates optimzied shader, by backing all used material effects into new shader file.
In optimized shader there are no optional controlers, like Enable/Disable antialiasing, lighting, changing gradient space and axis, etc.
Optimized shader has reduced file size and compiles faster.


Support thread:
http://forum.unity3d.com/threads/wireframe-the-amazing-wireframe-shader.251143/

 