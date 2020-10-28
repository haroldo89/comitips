using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Model
{
    interface IRepository<T> where T : class
    {
        IList<T> Get();
        T GetById(int? id);
        string InsertNew(T employee);
        string Update(T employee);
        string Delete(T employee);
    }
}
