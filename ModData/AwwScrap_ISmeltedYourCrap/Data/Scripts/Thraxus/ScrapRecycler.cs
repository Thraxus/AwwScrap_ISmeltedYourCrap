using Sandbox.Common.ObjectBuilders;
using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace Chipstix.ScrapRecycler
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_Refinery), false, "AwwScrapRecycler")]
    public class Scrap_Recycler : MyGameLogicComponent
 {
        private Dictionary<IMyModelDummy, MyParticleEffect> gasParticleEffects = new Dictionary<IMyModelDummy, MyParticleEffect>();
        private Dictionary<IMyModelDummy, MyParticleEffect> lazerParticleEffects = new Dictionary<IMyModelDummy, MyParticleEffect>();
        private Dictionary<string, MyEntitySubpart> subparts;
        private MyStringId material = MyStringId.GetOrCompute("DrugLaser");

        MyObjectBuilder_EntityBase objectBuilder = null;
        IMyRefinery ScrapRecycler = null;


        private int tick = 0;
        private float LazerLoop = 0.0f;

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            try
            {
                base.Init(objectBuilder);
                this.objectBuilder = objectBuilder;

                ScrapRecycler = Entity as IMyRefinery;

                NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME;
            }
            catch (Exception e)
            {
                if (tick % 100 == 0)
                {
                    MyVisualScriptLogicProvider.SendChatMessage("Init Error" + e, "DEBUG");
                }
                
            }
        }
        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return objectBuilder;
        }

        public override void UpdateAfterSimulation()
        {
            try
            {
                if (MyAPIGateway.Session == null)
                    return;

                var isHost = MyAPIGateway.Session.OnlineMode == MyOnlineModeEnum.OFFLINE ||
                             MyAPIGateway.Multiplayer.IsServer;

                var isDedicatedHost = isHost && MyAPIGateway.Utilities.IsDedicated;

                if (isDedicatedHost)
                    return;

                subparts = (ScrapRecycler as MyEntity)?.Subparts;

                // Check if Furnace is working and running.
                if (ScrapRecycler.IsProducing && ScrapRecycler.IsWorking)
	                RotateRings();
                UpdateParticleEffect();
            }
            
            catch (Exception e)
            {

                if (tick % 100 == 0)
                {
                    MyVisualScriptLogicProvider.SendChatMessage("Update Error" + e, "DEBUG");
                }
            }
        }

        public void RotateRings()
        {
            try
            {
                if (ScrapRecycler.IsWorking)
                {
                    if (subparts != null)
                    {
                        foreach (var subpart in subparts)
                        {
                            if (subpart.Key.Contains("Ring"))
                            {
                                var rotation = 0.01f;
                                var initialMatrix = subpart.Value.PositionComp.LocalMatrixRef;
                                var rotationMatrix = MatrixD.CreateRotationY(rotation);
                                Matrix matrix = rotationMatrix * initialMatrix;
                                subpart.Value.PositionComp.SetLocalMatrix(ref matrix);
                            }
                            else
                            {
                                if (tick % 100 == 0)
                                {
                                    MyVisualScriptLogicProvider.SendChatMessage("No ring subpart found", "DEBUG");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (tick % 100 == 0)
                {
                    MyVisualScriptLogicProvider.SendChatMessage("Animation Error" + e, "DEBUG");
                }
            }
        }

        private void UpdateParticleEffect()
        {
            MyParticleEffect lazerParticle = null;
            MyEntitySubpart subPart_ScanningCylinder = null;
            IMyModelDummy endPointDummy = null;

            if (ScrapRecycler.IsProducing && ScrapRecycler.IsWorking)
            {
                Dictionary<string, IMyModelDummy> ScrapRecyclerMainModelDummyList = new Dictionary<string, IMyModelDummy>();
                Dictionary<string, IMyModelDummy> scanningCylinderDummyList = new Dictionary<string, IMyModelDummy>();

                Entity.Model?.GetDummies(ScrapRecyclerMainModelDummyList);

                foreach (var subPart in subparts)
                {
                    if (subPart.Key.Contains("Ring"))
                    {
                        subPart_ScanningCylinder = subPart.Value;
                        
                    }
                }

                if (subPart_ScanningCylinder == null)
                {
                    if (tick % 100 == 0)
                    {
                        MyVisualScriptLogicProvider.SendChatMessage("No scanning cylinder found", "DEBUG");
                    }
                    return;
                }

                (subPart_ScanningCylinder?.Model as IMyModel)?.GetDummies(scanningCylinderDummyList);

                foreach (var dummy in ScrapRecyclerMainModelDummyList)
                {
                    if (dummy.Key.Contains("EndPoint"))
                        endPointDummy = dummy.Value;
                }

                if (endPointDummy == null)
                {
                    if (tick % 100 == 0)
                    {
                        MyVisualScriptLogicProvider.SendChatMessage("No end point dummy found", "DEBUG");
                    }
                    return;
                }
                
                MatrixD endPointMatrix = endPointDummy.Matrix * Entity.WorldMatrix;
                Vector3D endPointPosition = endPointMatrix.Translation;

                Dictionary<string, IMyModelDummy> ScrapRecyclerSubpartDummyList = new Dictionary<string, IMyModelDummy>();
                (subPart_ScanningCylinder.Model as IMyModel)?.GetDummies(ScrapRecyclerSubpartDummyList);

                foreach (var dummy in scanningCylinderDummyList)
                {
                    
                    if (dummy.Value == null || dummy.Key == null)
                        return;

                    MatrixD dummyMatrix = dummy.Value.Matrix * subPart_ScanningCylinder.WorldMatrix;
                    Vector3D dummyPosition = dummyMatrix.Translation;

                    
                    if (dummy.Key.Contains("particle"))
                    {
                        MyParticleEffect gasParticle = null;
                        if (!gasParticleEffects.ContainsKey(dummy.Value))
                        {
                            MyParticlesManager.TryCreateParticleEffect("FurnaceFire", ref dummyMatrix, ref dummyPosition, uint.MaxValue, out gasParticle);
                            gasParticle.UserScale = 0.75f;
                            //gasParticle.Loop = true;
                            gasParticleEffects.Add(dummy.Value, gasParticle);
                        }
                        else
                        {
                            gasParticleEffects.TryGetValue(dummy.Value, out gasParticle);
                            gasParticle.WorldMatrix = dummyMatrix;
                        }
                    }
                    //DrawLine(Color.Green, dummyPosition, endPointPosition);

                }

                LazerLoop++;
                if (LazerLoop == 100) LazerLoop = 0;
            }
            else
            {
                foreach (var particle in gasParticleEffects)
                {
                    particle.Value.Stop();
                    MyParticlesManager.RemoveParticleEffect(particle.Value);
                }
                foreach (var particle in lazerParticleEffects)
                {
                    particle.Value.Stop();
                    MyParticlesManager.RemoveParticleEffect(particle.Value);
                }
                
                gasParticleEffects.Clear();
                lazerParticleEffects.Clear();
            }
        }

        public void DrawLine(Color lineColor, Vector3D lineStartPoint, Vector3D lineHitVectorPoint, float length = 1.0f, float thickness = 0.01f)
        {
            MyTransparentGeometry.AddLineBillboard(material, lineColor, lineStartPoint, lineHitVectorPoint - lineStartPoint, length, thickness);
        }

        public override void Close()
        {
            foreach (var particle in gasParticleEffects)
            {
                particle.Value.Stop();
                MyParticlesManager.RemoveParticleEffect(particle.Value);
            }

            foreach (var particle in lazerParticleEffects)
            {
                particle.Value.Stop();
                MyParticlesManager.RemoveParticleEffect(particle.Value);
            }
        }
    }
}

