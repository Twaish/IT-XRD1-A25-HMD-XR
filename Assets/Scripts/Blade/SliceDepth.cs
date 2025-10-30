using UnityEngine;

public class SliceDepth : MonoBehaviour
{
  public int maxDepth = 3;
  public int currentDepth = 0;

  public bool CanBeSliced => currentDepth < maxDepth;

  public static void AssignTo(GameObject hull, SliceDepth parentDepth)
  {
    if (hull == null) return;

    SliceDepth newDepth = hull.AddComponent<SliceDepth>();

    if (parentDepth != null)
    {
      newDepth.maxDepth = parentDepth.maxDepth;
      newDepth.currentDepth = parentDepth.currentDepth + 1;
    }
  }
}