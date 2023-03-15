using System;
namespace Milo.Helper
{
    public interface OptionMenuSelectedListener
    {
        void OnOptionSelected(string selectedOption);

        void OnNothingSelected();
    }
}
