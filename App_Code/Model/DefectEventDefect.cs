public class DefectEventDefect : DefectEvent
{
	public DefectBase DEFECT;
	public DefectEventDefect() { }
	public DefectEventDefect(int id)
		: base(id)
	{
		DEFECT = new DefectBase(Defect.GetTTbyID(DEFECTID));
	}
}