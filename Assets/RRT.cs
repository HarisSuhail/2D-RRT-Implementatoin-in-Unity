using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RRT : MonoBehaviour
{
    public float xLow, xHigh, yLow, yHigh, stepSize, maxIterations, timeStep;
    public float zLevel, lineWidth;
    public bool step = false, disableStepping = false;
    public bool enableDrawing = true;
    private Tree theTree;
    private int counter = 0;
    private float timer = 0f;
    


 
    void Start()
    {

        theTree = new Tree(new Vector2(transform.position.x,transform.position.y));
        theTree.zL = zLevel;
        theTree.lWidth = lineWidth;
        theTree.enableDrawing = enableDrawing;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!disableStepping)
        {
            if (step)
            {
                timer = timeStep + 1f;
                step = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
        if (timer>timeStep && counter<maxIterations && !theTree.IsComplete())
        {
            Vector2 randPoint = GetRandomPoint();
            Debug.Log("Random Point" + randPoint.ToString());
            Node near = theTree.GetClosestLeaf(randPoint);
            Debug.Log("Nearest Node" + near.position.ToString());
            if (IsColliding(near.position, randPoint) != -1)
            {

                Vector2 direction = randPoint - near.position;
                float multiplier = (stepSize < direction.magnitude) ? stepSize : direction.magnitude;
                Node other = new Node(near.position + (direction.normalized * multiplier));
                //Node other = new Node(randPoint);
                Debug.Log("Node Added At " + other.position.ToString());
                theTree.AddLeaf(ref near, ref other);
                if (IsColliding(near.position, randPoint) == 1)
                {
                    //Node other = new Node(randPoint);
                    Debug.Log("COMPLETE");
                    other = theTree.AddLeaf(ref near, ref other);
                    theTree.AddFinalNode(ref other);
                    theTree.DrawCompletedPath();

                }
            }
            

            counter++;
            timer = 0;
        }
    }



  

    public class Tree
    {
        Node rootNode;
        public bool enableDrawing = true;
        private bool  complete = false;
        public float zL, lWidth;
        private float z2;
        private Node finalNode;
        private int count = 0;

        public void ViewParent(Node x)
        {
            Debug.Log("Parent is " + x.parent.position);
        }

        public Tree(Vector2 pos)
        {
            rootNode = new Node(pos);
           

        }
        public bool IsComplete()
        {
            return complete;
        }
        public void AddFinalNode( ref Node k)
        {
            finalNode = k;
          
            complete = true;
        }

        public void CreateAndAdd(Vector2 position)
        {
            Node nd = new Node(position);
            Node k = GetClosestLeaf(nd.position);
            AddLeaf(ref k,ref nd);
        }
        public Node AddLeaf( ref Node parentLeaf,ref Node childLeaf)
        {
            parentLeaf.AddChild( ref childLeaf);
            if (enableDrawing)
            {
                
                DrawLine(childLeaf.position, parentLeaf.position, Color.white);
            }
            return childLeaf;
        }

        public Node GetClosestLeaf(Vector2 pos)
        {
            return FindClosestInChildren(rootNode, pos);


        }
        private Node FindClosestInChildren(Node x, Vector2 target)
        {
            Node closest = x, temp;
            float closestDistance = Vector2.Distance(x.position,target);
            float checkDistance = 0f;
            if (x.children.Count != 0)
            {
                foreach (Node child in x.children)
                {
                    temp = FindClosestInChildren(child, target);
                    checkDistance = Vector2.Distance(temp.position, target);
                    if (checkDistance < closestDistance)
                    {
                        closestDistance = checkDistance;
                        closest = temp;
                    }
                }
            }
            return closest;
        }

        public void DrawCompletedPath()
        {
            Debug.Log("Drawing Completed Non Recursive");
            zL = zL - 1;
            DrawCompletedPath(finalNode);
            zL = zL + 1;
            Debug.Log("LINES DRAWN:" + count.ToString());
        }
        private void DrawCompletedPath(Node x)
        {
            Debug.Log("Drawing Completed Recursive");
            if (!complete)
            {
                 Debug.Log("Drawing Completed Recursive");
                return;
            }
            if (x.parent == null)
            {
                Debug.Log("Parent is Null");
                return;
            }
           
            count++;
            DrawLine(x.parent.position, x.position, Color.red);
            DrawCompletedPath(x.parent);
           

        }

        private void DrawLine(Vector2 _start, Vector2 _end, Color color)
        {
            Vector3 start = new Vector3(_start.x, _start.y, zL);
            Vector3 end = new Vector3(_end.x, _end.y, zL);

            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = lWidth;
            lr.endWidth = lWidth;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
           
        }

    }


    public class Node
    {
        public Vector2 position;
        public Node parent;
        public List<Node> children;
        public GameObject edgeLine;
        public float zL = -0.2f, lWidth = 0.1f;
        bool marked = false;

        public void DrawLine(Color color)
        {
            if (parent == null)
            {
                return;
            }
            edgeLine = new GameObject();
            Vector3 start = new Vector3(parent.position.x, parent.position.y, zL);
            Vector3 end = new Vector3(position.x, position.y, zL);


            edgeLine.transform.position = start;
            edgeLine.AddComponent<LineRenderer>();
            LineRenderer lr = edgeLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = lWidth;
            lr.endWidth = lWidth;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
        public Node()
        {
            parent = null;
            children = new List<Node>();
            position = new Vector2(0, 0);
            edgeLine = new GameObject();
        }
        public Node(Vector2 x)
        {
            parent = null;
            children = new List<Node>();
            position = x;
        }

        public void AddChild(ref Node x)
        {
            x.parent = this;
            children.Add(x);
            
             
        }

        public void SetParent(Node x)
        {
            parent = x;
        }

    }


    int IsColliding(Vector2 from, Vector2 to)
    {
        int temp = 0;

        RaycastHit2D hit = Physics2D.Raycast(from, to - from, stepSize);
        if (hit.collider != null)
        {

            if (hit.collider.gameObject.tag == "target")
            {
                temp = 1;
            }
            else
            {
                temp = -1;
            }

        }

        return temp;
    }

    Vector2 GetRandomPoint()
    {
        return new Vector2(Random.Range(xLow, xHigh), Random.Range(yLow, yHigh));
    }
}