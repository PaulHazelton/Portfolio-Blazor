namespace Portfolio;

public static class Routes
{
	public const string Home = "/";
	public const string ProjectList = "/projects";
	public const string Blogs = "/blogs";

	public static class Projects
	{
		public const string Recoil = $"{ProjectList}/recoil";
		public const string CpuSim = $"{ProjectList}/cpu-sim";
		public const string MonoGameResponsiveGui = $"{ProjectList}/monogame-responsive-gui";
		public const string Tilteroids = $"{ProjectList}/tilteroids";
	}
}