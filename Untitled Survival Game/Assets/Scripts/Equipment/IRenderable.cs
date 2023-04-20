using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRenderable
{
    public void SetMesh(Mesh mesh);

    public void SetMaterial(Material material);
}
