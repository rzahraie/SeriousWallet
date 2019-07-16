using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Widget;

namespace SeriousWallet3
{
    [Activity(Label = "NumberOfSignatoriesActivity")]
    public class NumberOfSignatoriesActivity : AppCompatActivity
    {
        int maxNumber = 6;
        int minNumber = 1;
        TextInputEditText numberTv;
        string prevValue;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.firsttimemain);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            Android.Views.LayoutInflater layoutInflater = this.LayoutInflater;

            Android.Views.View view = layoutInflater.Inflate(Resource.Layout.numberofsignatories, null, false);

            builder.SetView(view);

            ImageButton imp = view.FindViewById<ImageButton>(Resource.Id.plusButton);
            ImageButton imm = view.FindViewById<ImageButton>(Resource.Id.minusButton);
            
            imp.Click += Imp_Click;
            imm.Click += Imm_Click;

            numberTv = view.FindViewById<TextInputEditText>(Resource.Id.numberEdit);

            numberTv.Text = System.Convert.ToString(minNumber);

            numberTv.AfterTextChanged += Tv_AfterTextChanged;
            numberTv.BeforeTextChanged += NumberTv_BeforeTextChanged;

            //numberPicker.Value = SignatureRecoveryActivity.numberOfSignatories
            SignatureRecoveryActivity.CurrentSignatory = 1;
           

            builder.SetTitle("Number Of Signatories");

            builder.SetPositiveButton(Resource.String.next, (senderAlert, args) =>
            {
                SignatureRecoveryActivity.NumberOfSignatories = System.Convert.ToInt16(numberTv.Text);
                StartActivity(new Intent(Application.Context, typeof(SignatureRecoveryActivity)));
            });

            builder.SetNegativeButton(Resource.String.cancel, (senderAlert, args) => { Finish(); });

            builder.SetNeutralButton(Resource.String.prev, (senderAlert, args) =>
            {
                StartActivity(new Intent(Application.Context, typeof(PaperBackRecoveryActivity)));
            });

            builder.Show();
        }

        private void NumberTv_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            prevValue = numberTv.Text;
        }

        private void Tv_AfterTextChanged(object sender, Android.Text.AfterTextChangedEventArgs e)
        {
            int value = System.Convert.ToInt16(numberTv.Text);
            int iprevValue = System.Convert.ToInt16(prevValue);

            if ((value > maxNumber) && (value < minNumber))
            {
                numberTv.Text = prevValue;
                return;
            }

            SignatureRecoveryActivity.NumberOfSignatories = value;
        }

        private void Imm_Click(object sender, System.EventArgs e)
        {
            int value =  System.Convert.ToInt16(numberTv.Text);
            if (value >= minNumber) value--;
            numberTv.Text = System.Convert.ToString(value);
        }

        private void Imp_Click(object sender, System.EventArgs e)
        {
            int value = System.Convert.ToInt16(numberTv.Text);
            if (value < maxNumber) value++;
            numberTv.Text = System.Convert.ToString(value);
        }
    }
}