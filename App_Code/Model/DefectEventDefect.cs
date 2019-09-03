public class DefectEventDefect : DefectEvent
{
	public DefectBase DEFECT;
	public DefectEventDefect(int id)
		: base(id)
	{
		DEFECT = new DefectBase(Defect.GetTTbyID(DEFECTID));
	}
}