using System;
using System.Threading;

namespace ThreadedBinarySearchTree
{
    class Program
    {
        class Node
        {
            public int num { get; set; }
            public Node right { get; set; }
            public Node left { get; set; }

            public Node(int num)
            {
                this.num = num;
                this.right = null;
                this.left = null;
            }
        }
        class BinaryTree
        {
            public Node Root { get; set; }
            public Mutex mutexLock;
            
            public BinaryTree(Node root)
            {
                this.Root = root;
                this.mutexLock = new Mutex();

            }
            public BinaryTree()
            {
                this.Root = null;
                this.mutexLock = new Mutex();

            }
            public void add(int num)
            {
                Node n = new Node(num);
                
                mutexLock.WaitOne();
                if (this.Root == null)
                {
                    this.Root = n;
                    mutexLock.ReleaseMutex();
                    return;
                }

                Node current = this.Root;
                Node parent = null;
                while (current != null)
                {
                    parent = current;
                    if (num > current.num)
                    {
                        current = current.right;
                    }
                    else if (num < current.num)
                    {
                        current = current.left;
                    }
                    else {
                        mutexLock.ReleaseMutex();
                        return; }
                }
                if (parent.num < num)
                {
                    parent.right = n;
                }
                else if (parent.num > num)
                {
                    parent.left = n;
                }

                mutexLock.ReleaseMutex();
            }
            // remove num from the tree, if it doesn't exists, do nothing
            public void remove(int num) {
                mutexLock.WaitOne();
                Root = deleteNode(Root, num);
                mutexLock.ReleaseMutex();

            } 
            public Node deleteNode(Node node,int num)
            {
                mutexLock.WaitOne();
                if (node == null)
                {
                    mutexLock.ReleaseMutex();
                    return node;
                }
                if (num < node.num)
                {
                    node.left = deleteNode(node.left, num);
                }
                else if (num > node.num)
                {
                    node.right = deleteNode(node.right, num);
                }
                else
                {
                    if (node.right == null)
                    {
                        mutexLock.ReleaseMutex();
                        return node.left;
                    }
                    else if (node.left == null)
                    {
                        mutexLock.ReleaseMutex();
                        return node.right;
                    }
                    node.num = minValue(node.right);
                    node.right = deleteNode(node.right, node.num);
                    
                }
                mutexLock.ReleaseMutex();

                return node;
            }
            public int minValue(Node node)
            {
                mutexLock.WaitOne();
                int min = node.num;
                while (node.left != null)
                {
                    min = node.left.num;
                    node = node.left;
                }
                mutexLock.ReleaseMutex();
                return min;
            }

            public bool search(int num) {
                mutexLock.WaitOne();
                if (this.Root == null)
                {
                    mutexLock.ReleaseMutex();
                    return false;
                }
                Node curr = this.Root;
                while (curr!= null)
                {
                    if (curr.num == num)
                    {
                        mutexLock.ReleaseMutex();
                        return true;
                    }
                    else if (curr.num < num)
                    {
                        curr = curr.right;
                    }
                    else
                        curr = curr.left;

                }
                mutexLock.ReleaseMutex();
                return false;

                 } // search num in the tree and return true/false accordingly

            public void clear() {
                mutexLock.WaitOne();
                while (this.Root != null)
                {
                    this.Root = deleteNode(this.Root, this.Root.num);
                }
                mutexLock.ReleaseMutex();
            } // remove all items from tree

            public void print() {
                if (this.Root == null)
                    return;
                mutexLock.WaitOne();
                string s = "";
                s = printInOrder(this.Root,s);
                if(!s.Equals(""))
                    s = s.Substring(0, s.Length-1);
                Console.WriteLine(s);
                mutexLock.ReleaseMutex();
            }
            public string printInOrder(Node node , string s)
            {

                if (node == null)
                {
                    return s;
                }
                s = printInOrder(node.left,s);
                s += node.num + ",";
                s = printInOrder(node.right,s);
                return s;
            }
        }
    }
}
