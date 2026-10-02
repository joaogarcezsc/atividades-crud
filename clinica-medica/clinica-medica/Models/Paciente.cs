namespace ClinicaPOO.Models
{
    public class Paciente : Pessoa
    {
        public DateTime DataNascimento { get; set; }

        public Paciente(int codigo, string nome, string cpf, string telefone, DateTime dataNascimento)
            : base(codigo, nome, cpf, telefone)
        {
            DataNascimento = dataNascimento;
        }

        public override void ExibirFicha()
        {
            Console.WriteLine($"[PACIENTE] Cód: {Codigo} | Nome: {Nome} | CPF: {Cpf} | Data Nasc: {DataNascimento:dd/MM/yyyy}");
        }
    }
}