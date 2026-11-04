using Xunit;

namespace Gestion_alumnos.tests;

public class UnitTest1
{
    [Fact]
    public void SumarDosNumeros_DebeDevolverCinco()
    {
        // ARRANGE: preparar los datos
        int numero1 = 2;
        int numero2 = 3;

        // ACT: ejecutar la operación
        int resultado = numero1 + numero2;

        // ASSERT: comprobar el resultado
        Assert.Equal(5, resultado);
    }
}
