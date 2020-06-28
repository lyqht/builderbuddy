using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ModelKit
{
    public string name;
    public int parts;
    public int steps;
    public ModelKit(string name, int numParts, int steps) {
        this.name = name;
        this.parts = numParts;
        this.steps = steps;
    }
}