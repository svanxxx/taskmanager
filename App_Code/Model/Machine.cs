using System.Collections.Generic;
using System.Linq;

public class MachineWrapper
{
	public static void Delete(string name)
	{
		using (var db = new tt_resEntities())
		{
			var m = db.Machines.First(machine => machine.PCNAME == name);
			if (m != null)
			{
				db.Machines.Remove(m);
				db.SaveChanges();
			}
		}
	}
	public static List<Machine> Enum()
	{
		List<Machine> ls = new List<Machine>();
		using (var db = new tt_resEntities())
		{
			return db.Machines.ToList();
		}
	}
	public static Machine FindOrCreate(string name)
	{
		using (var db = new tt_resEntities())
		{
			var m = db.Machines.Find(name);
			if (m == null)
			{
				m = db.Machines.Create();
				m.PCNAME = name;
				db.Machines.Add(m);
				db.SaveChanges();
			}
			return m;
		}
	}
	public static Machine Find(string name)
	{
		using (var db = new tt_resEntities())
		{
			return db.Machines.Find(name);
		}
	}
	public static void Update(Machine m)
	{
		using (var db = new tt_resEntities())
		{
			db.Machines.Attach(m);
			db.Entry(m).State = System.Data.Entity.EntityState.Modified;
			db.SaveChanges();
		}
	}
}