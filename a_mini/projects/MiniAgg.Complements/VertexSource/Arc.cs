//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Arc vertex generator
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using MatterHackers.Agg;
using MatterHackers.VectorMath;

using FlagsAndCommand = MatterHackers.Agg.ShapePath.FlagsAndCommand;

namespace MatterHackers.Agg.VertexSource
{
    //=====================================================================arc
    //
    // See Implementation agg_arc.cpp 
    //
    public class Arc
    {
        double originX;
        double originY;

        double radiusX;
        double radiusY;

        double startAngle;
        double endAngle;
        double m_Scale;
        EDirection m_Direction;

        double m_CurrentFlatenAngle;
        double flatenDeltaAngle;

        bool m_IsInitialized;
        ShapePath.FlagsAndCommand m_NextPathCommand;

        public enum EDirection
        {
            ClockWise,
            CounterClockWise,
        }

        public Arc()
        {
            m_Scale = 1.0;
            m_IsInitialized = false;
        }

        public Arc(double OriginX, double OriginY,
             double RadiusX, double RadiusY,
             double Angle1, double Angle2)
            : this(OriginX, OriginY, RadiusX, RadiusY, Angle1, Angle2, EDirection.CounterClockWise)
        {

        }

        public Arc(double OriginX, double OriginY,
             double RadiusX, double RadiusY,
             double Angle1, double Angle2,
             EDirection Direction)
        {
            originX = OriginX;
            originY = OriginY;
            radiusX = RadiusX;
            radiusY = RadiusY;
            m_Scale = 1.0;
            Normalize(Angle1, Angle2, Direction);
        }

        public void Init(double OriginX, double OriginY,
                  double RadiusX, double RadiusY,
                  double Angle1, double Angle2)
        {
            Init(OriginX, OriginY, RadiusX, RadiusY, Angle1, Angle2, EDirection.CounterClockWise);
        }

        public void Init(double OriginX, double OriginY,
                   double RadiusX, double RadiusY,
                   double Angle1, double Angle2,
                   EDirection Direction)
        {
            originX = OriginX;
            originY = OriginY;
            radiusX = RadiusX;
            radiusY = RadiusY;
            Normalize(Angle1, Angle2, Direction);
        }

        public double ApproximateScale 
        {
            get{ return this.m_Scale;}
            set
            {
                m_Scale = value;
                if (m_IsInitialized)
                {
                    Normalize(startAngle, endAngle, m_Direction);
                }
            }
        }
         
        public IEnumerable<VertexData> GetVertexIter()
        {
            // go to the start
            VertexData vertexData = new VertexData();
            vertexData.command = FlagsAndCommand.CommandMoveTo;
            vertexData.x = originX + Math.Cos(startAngle) * radiusX;
            vertexData.y = originY + Math.Sin(startAngle) * radiusY;
            yield return vertexData;

            double angle = startAngle;
            vertexData.command = FlagsAndCommand.CommandLineTo;
            while ((angle < endAngle - flatenDeltaAngle / 4) == (((int)EDirection.CounterClockWise) == 1))
            {
                angle += flatenDeltaAngle;

                vertexData.x = originX + Math.Cos(angle) * radiusX;
                vertexData.y = originY + Math.Sin(angle) * radiusY;
                yield return vertexData;
            }

            vertexData.x = originX + Math.Cos(endAngle) * radiusX;
            vertexData.y = originY + Math.Sin(endAngle) * radiusY;
            yield return vertexData;

            vertexData.command = FlagsAndCommand.CommandStop;
            yield return vertexData;
        }

        //public void RewindZero()
        //{
        //    m_NextPathCommand = ShapePath.FlagsAndCommand.CommandMoveTo;
        //    m_CurrentFlatenAngle = startAngle;
        //}

        //public ShapePath.FlagsAndCommand vertex(out double x, out double y)
        //{
        //    x = 0;
        //    y = 0;

        //    if (ShapePath.IsStop(m_NextPathCommand))
        //    {
        //        return ShapePath.FlagsAndCommand.CommandStop;
        //    }

        //    if ((m_CurrentFlatenAngle < endAngle - flatenDeltaAngle / 4) != ((int)EDirection.CounterClockWise == 1))
        //    {
        //        x = originX + Math.Cos(endAngle) * radiusX;
        //        y = originY + Math.Sin(endAngle) * radiusY;
        //        m_NextPathCommand = ShapePath.FlagsAndCommand.CommandStop;

        //        return ShapePath.FlagsAndCommand.CommandLineTo;
        //    }

        //    x = originX + Math.Cos(m_CurrentFlatenAngle) * radiusX;
        //    y = originY + Math.Sin(m_CurrentFlatenAngle) * radiusY;

        //    m_CurrentFlatenAngle += flatenDeltaAngle;

        //    ShapePath.FlagsAndCommand CurrentPathCommand = m_NextPathCommand;
        //    m_NextPathCommand = ShapePath.FlagsAndCommand.CommandLineTo;
        //    return CurrentPathCommand;
        //}

        private void Normalize(double Angle1, double Angle2, EDirection Direction)
        {
            double ra = (Math.Abs(radiusX) + Math.Abs(radiusY)) / 2;
            flatenDeltaAngle = Math.Acos(ra / (ra + 0.125 / m_Scale)) * 2;
            if (Direction == EDirection.CounterClockWise)
            {
                while (Angle2 < Angle1)
                {
                    Angle2 += Math.PI * 2.0;
                }
            }
            else
            {
                while (Angle1 < Angle2)
                {
                    Angle1 += Math.PI * 2.0;
                }
                flatenDeltaAngle = -flatenDeltaAngle;
            }
            m_Direction = Direction;
            startAngle = Angle1;
            endAngle = Angle2;
            m_IsInitialized = true;
        }
    }
}
