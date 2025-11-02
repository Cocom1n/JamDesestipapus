
public interface IDesactivarMovimiento
{
    /// <summary>
    /// Desactiva el movimiento durante <paramref name="duracion"/> segundos.
    /// La implementación debe garantizar que el objeto deje de sobrescribir la velocidad horizontal.
    /// </summary>
    void DesactivarMovimiento(float duracion);

    /// <summary>
    /// Reactiva inmediatamente el movimiento si estaba desactivado.
    /// </summary>
    void ReactivarMovimiento();
}