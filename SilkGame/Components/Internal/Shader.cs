using System;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;

namespace SilkGame
{
    public class Shader : IDisposable
    {
        private uint handle;
        private GL gl;
        private bool ignoreUniformsNotFound;

        public Shader(GL glContext, string name, bool ignoreUniformsNotFound = false) : 
            this(glContext, $"Shaders/{name}.vert", $"Shaders/{name}.frag", ignoreUniformsNotFound)
        {
            
        }
        public Shader(GL glContext, string vertexPath, string fragmentPath, bool ignoreUniformsNotFound = false)
        {
            gl = glContext;
            
            uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
            handle = gl.CreateProgram();
            gl.AttachShader(handle, vertex);
            gl.AttachShader(handle, fragment);
            gl.LinkProgram(handle);
            gl.GetProgram(handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {gl.GetProgramInfoLog(handle)}");
            }
            gl.DetachShader(handle, vertex);
            gl.DetachShader(handle, fragment);
            gl.DeleteShader(vertex);
            gl.DeleteShader(fragment);
            this.ignoreUniformsNotFound = ignoreUniformsNotFound;
        }

        public void SetAsCurrentGLProgram()
        {
            gl.UseProgram(handle);
        }

        public void SetUniform(string name, int value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                if (ignoreUniformsNotFound)
                    return;
                throw new Exception($"{name} uniform not found on shader.");
            }
            gl.Uniform1(location, value);
        }
        

        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            //A new overload has been created for setting a uniform so we can use the transform in our shader.
            int location = gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                if (ignoreUniformsNotFound)
                    return;
                throw new Exception($"{name} uniform not found on shader.");
            }
            gl.UniformMatrix4(location, 1, false, (float*) &value);
        }

        public void SetUniform(string name, float value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                if (ignoreUniformsNotFound)
                    return;
                throw new Exception($"{name} uniform not found on shader.");
            }
            gl.Uniform1(location, value);
        }
        public void SetUniform(string name, Vector2 value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                if (ignoreUniformsNotFound)
                    return;
                throw new Exception($"{name} uniform not found on shader.");
            }
            gl.Uniform2(location, value);
        }
        public void SetUniform(string name, Vector3 value)
        {
            int location = gl.GetUniformLocation(handle, name);
            if (location == -1)
            {
                if (ignoreUniformsNotFound)
                    return;
                throw new Exception($"{name} uniform not found on shader.");
            }
            gl.Uniform3(location, value);
        }
        public void Dispose()
        {
            gl.DeleteProgram(handle);
        }

        private uint LoadShader(ShaderType type, string path)
        {
            string src = File.ReadAllText(path);
            uint handle = gl.CreateShader(type);
            gl.ShaderSource(handle, src);
            gl.CompileShader(handle);
            string infoLog = gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
    }
}
