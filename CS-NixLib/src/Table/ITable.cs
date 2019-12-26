using System;

namespace Nixill.Tables
{
  public interface ITable<T>
  {
    public abstract int Height { get; }
    public abstract int Width { get; }
    public abstract int Size { get; }
    public abstract int Area { get; }


  }
}