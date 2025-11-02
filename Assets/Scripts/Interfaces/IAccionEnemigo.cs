public interface IAccionEnemigo
{
    //Iniciar la accion
    void EjecutarAccion(UnityEngine.Transform jugador, UnityEngine.Rigidbody2D rb);
    //Detener la accion
    void DetenerAccion();
    //Indicar si la accion esta corriendo
    bool EstaEjecutandoAccion();
}
