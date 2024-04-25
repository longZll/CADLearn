using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;

namespace PointOnCurveTest
{
  public class PtTransOverrule : TransformOverrule
  {
    // A static pointer to our overrule instance

    static public PtTransOverrule theOverrule =
      new PtTransOverrule();

    // A list of the curves that have had points
    // attached to

    static internal List<ObjectId> _curves =
      new List<ObjectId>();

    // A flag to indicate whether we're overruling

    static bool overruling = false;

    public PtTransOverrule() {}

    // Out primary overruled function

    public override void TransformBy(Entity e, Matrix3d mat)
    {
      // We only care about points

      DBPoint pt = e as DBPoint;
      if (pt != null)
      {
        Database db = HostApplicationServices.WorkingDatabase;

        // Work through the curves to find the closest to our
        // transformed point

        double min = 0.0;
        Point3d bestPt = Point3d.Origin;
        bool first = true;

        // We're using an Open/Close transaction, to avoid
        // problems with us using transactions in an event
        // handler

        OpenCloseTransaction tr =
          db.TransactionManager.StartOpenCloseTransaction();
        using (tr)
        {
          foreach (ObjectId curId in _curves)
          {
            DBObject obj =
              tr.GetObject(curId, OpenMode.ForRead);
            Curve cur = obj as Curve;
            if (cur != null)
            {
              Point3d ptLoc =
                pt.Position.TransformBy(mat);
              Point3d ptOnCurve =
                cur.GetClosestPointTo(ptLoc, false);
              Vector3d dist = ptOnCurve - ptLoc;

              if (first || dist.Length < min)
              {
                first = false;
                min = dist.Length;
                bestPt = ptOnCurve;
              }
            }
          }
          pt.Position = bestPt;
        }
      }
    }

    [CommandMethod("POC")]
    public void CreatePointOnCurve()
    {
      Document doc =
        Application.DocumentManager.MdiActiveDocument;
      Database db = doc.Database;
      Editor ed = doc.Editor;

    // Ask the user to select a curve

      PromptEntityOptions opts =
      new PromptEntityOptions(
        "\nSelect curve at the point to create: "
      );
      opts.SetRejectMessage(
        "\nEntity must be a curve."
      );
      opts.AddAllowedClass(typeof(Curve), false);

      PromptEntityResult per = ed.GetEntity(opts);
    
      ObjectId curId = per.ObjectId;
      if (curId != ObjectId.Null)
      {
      // Let's make sure we'll be able to see our point

      db.Pdmode = 97;  // square with a circle
      db.Pdsize = -10; // relative to the viewport size

        Transaction tr =
          doc.TransactionManager.StartTransaction();
        using (tr)
        {
          DBObject obj =
            tr.GetObject(curId, OpenMode.ForRead);
          Curve cur = obj as Curve;
          if (cur != null)
          {
          // Our initial point should be the closest point
          // on the curve to the one picked

            Point3d pos =
              cur.GetClosestPointTo(per.PickedPoint, false);
            DBPoint pt = new DBPoint(pos);

          // Add it to the same space as the curve

            BlockTableRecord btr =
              (BlockTableRecord)tr.GetObject(
                cur.BlockId,
                OpenMode.ForWrite
              );
            ObjectId ptId = btr.AppendEntity(pt);
            tr.AddNewlyCreatedDBObject(pt, true);
          }
          tr.Commit();
        
        // And add the curve to our central list
        
          _curves.Add(curId);
        }

      // Turn on the transform overrule if it isn't already

        if (!overruling)
        {
          ObjectOverrule.AddOverrule(
            RXClass.GetClass(typeof(DBPoint)),
          PtTransOverrule.theOverrule,
            true
          );
          overruling = true;
          TransformOverrule.Overruling = true;
        }
      }
    }
  }
}