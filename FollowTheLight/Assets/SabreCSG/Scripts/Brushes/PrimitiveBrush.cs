#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sabresaurus.SabreCSG
{
    public enum PrimitiveBrushType { Cube, Sphere, Cylinder, Prism };

    [ExecuteInEditMode]
    public class PrimitiveBrush : Brush
    {
        //		[SerializeField]
        //	    protected PrimitiveBrushType primitiveBrushType = PrimitiveBrushType.Cube;

        [SerializeField]
		Polygon[] polygons;

        // Maps triangle index (input) to polygon index (output). i.e. int polyIndex = polygonIndices[triIndex];
        List<int> polygonIndices;

		[SerializeField]
		int prismSideCount = 6;

		[SerializeField]
		PrimitiveBrushType brushType = PrimitiveBrushType.Cube;

	    private CSGModel parentCsgModel;

		public Polygon[] Polygons
        {
            get
            {
                return polygons;
            }
        }

		public PrimitiveBrushType BrushType {
			get {
				return brushType;
			}
			set {
				brushType = value;
			}
		}

		public void SetPolygons(Polygon[] polygons)
        {
            this.polygons = polygons;
            Invalidate();
        }

        void Start()
        {
			EnsureWellFormed();

            // Make sure the CSG Model knows about this brush. If they duplicated a brush in the hierarchy then this
            // allows us to make sure the CSG Model knows about it
            

			Invalidate();

            EditorUtility.SetSelectedWireframeHidden(GetComponent<Renderer>(), true);
        }

		public void ResetPolygons()
		{
			if (brushType == PrimitiveBrushType.Cube)
			{
				polygons = PolygonFactory.GenerateCube();
			}
			else if (brushType == PrimitiveBrushType.Cylinder)
			{
				polygons = PolygonFactory.GenerateCylinder();
			}
			else if (brushType == PrimitiveBrushType.Sphere)
			{
				polygons = PolygonFactory.GenerateSphere();
			}
			else if (brushType == PrimitiveBrushType.Prism)
			{
				if(prismSideCount < 3)
				{
					prismSideCount = 3;
				}
				polygons = PolygonFactory.GeneratePrism(prismSideCount);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		void DrawPolygons(Color color, params Polygon[] polygons)
		{
			GL.Begin(GL.TRIANGLES);
			color.a = 0.7f;
			GL.Color(color);
			
			for (int j = 0; j < polygons.Length; j++) 
			{
				Polygon polygon = polygons[j];
				Vector3 position1 = polygon.Vertices[0].Position;
				
				for (int i = 1; i < polygon.Vertices.Length - 1; i++)
				{
					GL.Vertex(transform.TransformPoint(position1));
					GL.Vertex(transform.TransformPoint(polygon.Vertices[i].Position));
					GL.Vertex(transform.TransformPoint(polygon.Vertices[i + 1].Position));
				}
			}
			GL.End();
		}

        public void OnRepaint(SceneView sceneView, Event e)
        {
            // Selected brush green outline
			if(!isBrushConvex)
			{
				SabreGraphics.GetSelectedBrushMaterial().SetPass(0);
				DrawPolygons(Color.red, polygons);
			}
        }

		public override Polygon[] GenerateTransformedPolygons()
		{
			Polygon[] polygonsCopy = polygons.DeepCopy<Polygon>();

			Vector3 center = transform.position;
			Quaternion rotation = transform.rotation;
			Vector3 scale = transform.localScale;

			for (int i = 0; i < polygons.Length; i++)
			{
				for (int j = 0; j < polygons[i].Vertices.Length; j++)
				{
					polygonsCopy[i].Vertices[j].Position = rotation * polygonsCopy[i].Vertices[j].Position.Multiply(scale) + center;
					polygonsCopy[i].Vertices[j].Normal = rotation * polygonsCopy[i].Vertices[j].Normal;
				}

				// Just updated a load of vertex positions, so make sure the cached plane is updated
				polygonsCopy[i].CalculatePlane();

//				// If the polygon doesn't have a material override use the brush's material
//				if(polygonsCopy[i].Material == null)
//				{
//					polygonsCopy[i].Material = this.material;
//				}


			}
			return polygonsCopy;
		}

		// Fired by the CSG Model
        public override void OnUndoRedoPerformed()
        {
            Invalidate();
        }

        void EnsureWellFormed()
        {
            if (polygons == null || polygons.Length == 0)
            {
				ResetPolygons();
            }
        }

//        public void OnDrawGizmosSelected()
//        {
//            // Ensure Edit Mode is on
//            GetCSGModel().EditMode = true;
//        }
//
//        public void OnDrawGizmos()
//        {
//            EnsureWellFormed();
//
//            //			Gizmos.color = Color.green;
//            //			for (int i = 0; i < PolygonFactory.hackyDisplay1.Count; i++) 
//            //			{
//            //				Gizmos.DrawSphere(PolygonFactory.hackyDisplay1[i], 0.2f);
//            //			}
//            //
//            //			Gizmos.color = Color.red;
//            //			for (int i = 0; i < PolygonFactory.hackyDisplay2.Count; i++) 
//            //			{
//            //				Gizmos.DrawSphere(PolygonFactory.hackyDisplay2[i], 0.2f);
//            //			}
//        }


	    public CSGModel GetCSGModel()
	    {
	        if (parentCsgModel == null)
	        {
	            parentCsgModel = transform.GetComponentInParent<CSGModel>();
	        }
	        return parentCsgModel;
	    }

		void OnEnable()
		{
			bool newBrush = GetCSGModel().TrackBrush(this);
			
			if(newBrush)
			{
				MeshFilter meshFilter = gameObject.AddOrGetComponent<MeshFilter>();
				MeshCollider meshCollider = gameObject.AddOrGetComponent<MeshCollider>();
				
				meshFilter.sharedMesh = new Mesh();
				meshCollider.sharedMesh = new Mesh();
			}

//			Invalidate();
		}

        public override void Invalidate()
        {
			if(!gameObject.activeInHierarchy)
			{
				return;
			}
			// Make sure there is a mesh filter on this object
			MeshFilter meshFilter = gameObject.AddOrGetComponent<MeshFilter>();
			MeshCollider meshCollider = gameObject.AddOrGetComponent<MeshCollider>();
			MeshRenderer meshRenderer = gameObject.AddOrGetComponent<MeshRenderer>();
			
			MeshCollider[] meshColliders = GetComponents<MeshCollider>();

			if(meshColliders.Length > 1)
			{
				for (int i = 1; i < meshColliders.Length; i++) 
				{
					DestroyImmediate(meshColliders[i]);
				}
			} 

			Mesh renderMesh = meshFilter.sharedMesh;
            PolygonFactory.GenerateMeshFromPolygons(polygons, ref renderMesh, out polygonIndices);

			// Displace the triangles for display along the normals very slightly (this is so we can overlay built
			// geometry with semi-transparent geometry and avoid depth fighting)
            


			Mesh collisionMesh = meshCollider.sharedMesh;

			if(renderMesh == collisionMesh)
			{
				collisionMesh = new Mesh();
			}

			collisionMesh.Clear();
			collisionMesh.vertices = renderMesh.vertices;
			collisionMesh.normals = renderMesh.normals;
			collisionMesh.triangles = renderMesh.triangles;

			if(mode == CSGMode.Subtract)
			{
				PolygonFactory.Invert(ref renderMesh);
			}
			PolygonFactory.Displace(ref renderMesh, 0.001f);

			meshFilter.sharedMesh = renderMesh;
			// For some reason have to set the collider mesh to null first to force it to update
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = collisionMesh;
				
			meshRenderer.receiveShadows = false;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			meshFilter.hideFlags = HideFlags.NotEditable;// | HideFlags.HideInInspector;
			meshRenderer.hideFlags = HideFlags.NotEditable;// | HideFlags.HideInInspector;

			meshRenderer.sharedMaterial = AssetDatabase.LoadMainAssetAtPath("Assets/SabreCSG/Materials/" + this.mode.ToString() + ".mat") as Material;

			isBrushConvex = PolygonFactory.IsMeshConvex(polygons);
        }

        public override void SetVisible(bool visible)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = visible;
            }
        }

		public Polygon GetPolygonFromTriangle(int triangleIndex)
        {
            int polygonIndex = polygonIndices[triangleIndex];
            return polygons[polygonIndex];
        }

        public override Bounds GetBounds()
        {
			if (polygons.Length > 0)
			{
				Bounds bounds = new Bounds(polygons[0].Vertices[0].Position, Vector3.zero);
				
				for (int i = 0; i < polygons.Length; i++)
				{
					for (int j = 0; j < polygons[i].Vertices.Length; j++)
					{
						bounds.Encapsulate(polygons[i].Vertices[j].Position);
					}
				}
				return bounds;
			}
			else
			{
				return new Bounds(Vector3.zero, Vector3.zero);
			}
        }

		public override int[] GetPolygonIDs ()
		{
			int[] ids = new int[polygons.Length];
			for (int i = 0; i < polygons.Length; i++) 
			{
				ids[i] = polygons[i].UniqueIndex;
			}
			return ids;
		}

		public override Polygon[] GetPolygons ()
		{
			return polygons;
		}

		public override int AssignUniqueIDs (int startingIndex, ref Dictionary<int, Polygon> idPolygonMappings)
		{
			int assignedCount = 0;
			if(idPolygonMappings != null)
			{
				// This call is expensive, but it may cause issues if removed
				Polygon[] transformedPolygons = GenerateTransformedPolygons();

				for (int i = 0; i < polygons.Length; i++) 
				{
					int uniqueIndex = startingIndex + i;
					polygons[i].UniqueIndex = uniqueIndex;
					transformedPolygons[i].UniqueIndex = uniqueIndex;
					if(mode == CSGMode.Subtract)
					{
						transformedPolygons[i].Flip();
					}
					idPolygonMappings[startingIndex + i] = transformedPolygons[i];
				}
			}
			else
			{
				for (int i = 0; i < polygons.Length; i++) 
				{
					int uniqueIndex = startingIndex + i;
					polygons[i].UniqueIndex = uniqueIndex;
				}
			}

			assignedCount = polygons.Length;
			
			return assignedCount;
		}

		public void ResetPivot()
		{
			Vector3 delta = GetBounds().center;

			for (int i = 0; i < polygons.Length; i++) 
			{
				for (int j = 0; j < polygons[i].Vertices.Length; j++) 
				{
					polygons[i].Vertices[j].Position -= delta;
				}
			}

			// Bounds is aligned with the object
			transform.Translate(delta);

			Invalidate();
		}

		public void Rescale (float rescaleValue)
		{
			for (int i = 0; i < polygons.Length; i++) 
			{
				for (int j = 0; j < polygons[i].Vertices.Length; j++) 
				{
					polygons[i].Vertices[j].Position *= rescaleValue;
					polygons[i].Vertices[j].UV *= rescaleValue;
				}
			}
			Invalidate();
		}

		public void RecalculateNormals()
		{
			for (int i = 0; i < polygons.Length; i++) 
			{
				polygons[i].ResetVertexNormals();
			}
			Invalidate();
		}

		public GameObject Duplicate()
		{
			// Duplicate the game object, copying serialized properties
			GameObject newObject = new GameObject(this.gameObject.name);
			EditorUtility.CopySerialized(this.gameObject, newObject);
			
			// For some reason we can't use CopySerialized on the transform, so copy properties manually
			newObject.transform.parent = this.transform.parent;
			newObject.transform.localPosition = this.transform.localPosition;
			newObject.transform.localRotation = this.transform.localRotation;
			newObject.transform.localScale = this.transform.localScale;
			
			// Now that the parent has been set we can set up a primitive brush on the object (parent must
			// be correct so that PrimitiveBrush.Awake fetches the right CSG model)
			newObject.AddComponent<PrimitiveBrush>();
			// Copy all the brush settings across
			EditorUtility.CopySerialized(this, newObject.GetComponent<PrimitiveBrush>());
			
			return newObject;
		}
    }
}

#endif