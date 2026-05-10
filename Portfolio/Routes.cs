namespace Portfolio;

public static class Routes
{
	public const string Home = "/";
	public const string ProjectList = "/projects";
	public const string BlogList = "/blogs";

	public const string Blog = "/blog/{fileName:nonfile}";
	public static string LinkToBlog(string fileName) => $"/blog/{fileName}";

	public static class Projects
	{
		public const string Recoil = $"{ProjectList}/recoil";
		public const string ZoomNotify = $"{ProjectList}/zoom-notify";
		public const string CpuSim = $"{ProjectList}/cpu-sim";
		public const string MonoGameResponsiveGui = $"{ProjectList}/monogame-responsive-gui";
		public const string Tilteroids = $"{ProjectList}/tilteroids";
	}
}