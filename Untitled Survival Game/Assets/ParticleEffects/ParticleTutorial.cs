using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTutorial : MonoBehaviour
{


    // Four key parameters Duration, Start lifetime, start delay, and start speed
    // Duration is the amount of time emmission lasts (importantly it is not the time the effect lasts)

    // Start Lifetime is the time each particle lasts after being emmitted (total length of the effect is close to Duration + Start Lifetime)
    // This doesnt have to be a single number but can be set to a random value between ranges or driven by curves (options set in drop down)

    // Start Speed is the initial speed particles are given when moving outwards or inwards (inwards can be done with negative speed)
    // Start Delay the amount of time to wait before starting to emit (usefull if you want to trigger an animation and have particles start in the middle)

    // Emission Tab
    // Rate over time: particles emitted per second
    // Bursts: can be used to emit a number of particles in a single burst (set Rate over time to zero to get only the bursts)


    // It is common to a have a parent particle system the controls (and triggers) the effect and have all the different parts as child particle systems

    // Particles don't have to be static images they can be animated images, or 3d meshes
}
