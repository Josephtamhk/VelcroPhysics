﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Controllers
{
    public enum GravityType
    {
        Linear,
        DistanceSquared
    }

    public class GravityController : Controller
    {
        public List<Vector2> Points = new List<Vector2>();
        public List<Body> Bodies = new List<Body>();
        public float MinRadius { get; set; }
        public float MaxRadius { get; set; }
        public float Strength { get; set; }
        public GravityType GravityType { get; set; }

        public GravityController(float strength)
        {
            Strength = strength;
            MaxRadius = float.MaxValue;
        }

        public GravityController(float strength, float maxRadius, float minRadius)
        {
            MinRadius = minRadius;
            MaxRadius = maxRadius;
            Strength = strength;
        }

        public override void Update()
        {
            Vector2 f = Vector2.Zero;

            for (Body body1 = World.BodyList; body1 != null; body1 = body1.NextBody)
            {
                if (!body1.Enabled || body1.IgnoreGravity || body1.IsStatic)
                    continue;

                foreach (Body body2 in Bodies)
                {
                    if (body1 == body2 || (body1.IsStatic && body2.IsStatic) || !body2.Enabled)
                        continue;

                    Vector2 d = body2.Position - body1.Position;
                    float r2 = d.LengthSquared();

                    if (r2 < Settings.Epsilon)
                        continue;

                    float r = d.Length();

                    if (r >= MaxRadius || r <= MinRadius)
                        continue;

                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            f = Strength / r2 / (float)Math.Sqrt(r2) * body1.Mass * body2.Mass * d;
                            break;
                        case GravityType.Linear:
                            f = Strength / r2 * body1.Mass * body2.Mass * d;
                            break;
                    }

                    body1.ApplyForce(ref f);
                    Vector2.Negate(ref f, out f);
                    body2.ApplyForce(ref f);
                }

                foreach (Vector2 point in Points)
                {
                    Vector2 d = point - body1.Position;
                    float r2 = d.LengthSquared();

                    if (r2 < Settings.Epsilon)
                        continue;

                    float r = d.Length();

                    if (r >= MaxRadius || r <= MinRadius)
                        continue;

                    switch (GravityType)
                    {
                        case GravityType.DistanceSquared:
                            f = Strength / r2 / (float)Math.Sqrt(r2) * body1.Mass * d;
                            break;
                        case GravityType.Linear:
                            f = Strength / r2 * body1.Mass * d;
                            break;
                    }

                    body1.ApplyForce(ref f);
                }
            }
        }

        public void AddBody(Body body)
        {
            Bodies.Add(body);
        }

        public void AddPoint(Vector2 point)
        {
            Points.Add(point);
        }
    }
}
