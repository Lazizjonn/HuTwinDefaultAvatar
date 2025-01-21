using UnityEngine;

public class HostShareScript : MonoBehaviour
{
    [SerializeField] GameObject hostShare1, hostShare2, hostShare3, hostShare4, hostShare5,
        hostShare6, hostShare7, hostShare8, hostShare9, hostShare10, hostShare11;

    public void SetFaceData(float[] data)
    {
        this.transform.localPosition = new Vector3(data[0] * 10000000, data[1] * 10000000, data[2] * 10000000);
        this.transform.localScale = new Vector3(data[3] * 10000000, data[4] * 10000000, data[5] * 10000000);

        hostShare1.transform.localPosition = new Vector3(data[6] * 10000000, data[7] * 10000000, data[8] * 10000000);
        hostShare1.transform.localScale = new Vector3(data[9] * 10000000, data[10] * 10000000, data[11] * 10000000);

        hostShare2.transform.localPosition = new Vector3(data[12] * 10000000, data[13] * 10000000, data[14] * 10000000);
        hostShare2.transform.localScale = new Vector3(data[15] * 10000000, data[16] * 10000000, data[17] * 10000000);

        hostShare3.transform.localPosition = new Vector3(data[18] * 10000000, data[19] * 10000000, data[20] * 10000000);
        hostShare3.transform.localScale = new Vector3(data[21] * 10000000, data[22] * 10000000, data[23] * 10000000);

        hostShare4.transform.localPosition = new Vector3(data[24] * 10000000, data[25] * 10000000, data[26] * 10000000);
        hostShare4.transform.localScale = new Vector3(data[27] * 10000000, data[28] * 10000000, data[29] * 10000000);

        hostShare5.transform.localPosition = new Vector3(data[30] * 10000000, data[31] * 10000000, data[32] * 10000000);
        hostShare5.transform.localScale = new Vector3(data[33] * 10000000, data[34] * 10000000, data[35] * 10000000);

        hostShare6.transform.localPosition = new Vector3(data[36] * 10000000, data[37] * 10000000, data[37] * 10000000);
        hostShare6.transform.localScale = new Vector3(data[39] * 10000000, data[40] * 10000000, data[41] * 10000000);

        hostShare7.transform.localPosition = new Vector3(data[42] * 10000000, data[43] * 10000000, data[44] * 10000000);
        hostShare7.transform.localScale = new Vector3(data[45] * 10000000, data[46] * 10000000, data[47] * 10000000);

        hostShare8.transform.localPosition = new Vector3(data[48] * 10000000, data[49] * 10000000, data[50] * 10000000);
        hostShare8.transform.localScale = new Vector3(data[15] * 10000000, data[51] * 10000000, data[52] * 10000000);

        hostShare9.transform.localPosition = new Vector3(data[53] * 10000000, data[54] * 10000000, data[55] * 10000000);
        hostShare9.transform.localScale = new Vector3(data[56] * 10000000, data[57] * 10000000, data[58] * 10000000);

        hostShare10.transform.localPosition = new Vector3(data[59] * 10000000, data[60] * 10000000, data[61] * 10000000);
        hostShare10.transform.localScale = new Vector3(data[62] * 10000000, data[63] * 10000000, data[64] * 10000000);

        hostShare11.transform.localPosition = new Vector3(data[65] * 10000000, data[66] * 10000000, data[67] * 10000000);
        hostShare11.transform.localScale = new Vector3(data[68] * 10000000, data[69] * 10000000, 0);
    }

    public float[] GetFaceData()
    {
        return new float[]{
            0.0000001f * this.transform.localPosition.x,
            0.0000001f * this.transform.localPosition.y,
            0.0000001f * this.transform.localPosition.z,
            0.0000001f * this.transform.localScale.x,
            0.0000001f * this.transform.localScale.y,
            0.0000001f * this.transform.localScale.z,

            0.0000001f * hostShare1.transform.localPosition.x,
            0.0000001f * hostShare1.transform.localPosition.y,
            0.0000001f * hostShare1.transform.localPosition.z,
            0.0000001f * hostShare1.transform.localScale.x,
            0.0000001f * hostShare1.transform.localScale.y,
            0.0000001f * hostShare1.transform.localScale.z,

            0.0000001f * hostShare2.transform.localPosition.x,
            0.0000001f * hostShare2.transform.localPosition.y,
            0.0000001f * hostShare2.transform.localPosition.z,
            0.0000001f * hostShare2.transform.localScale.x,
            0.0000001f * hostShare2.transform.localScale.y,
            0.0000001f * hostShare2.transform.localScale.z,

            0.0000001f * hostShare3.transform.localPosition.x,
            0.0000001f * hostShare3.transform.localPosition.y,
            0.0000001f * hostShare3.transform.localPosition.z,
            0.0000001f * hostShare3.transform.localScale.x,
            0.0000001f * hostShare3.transform.localScale.y,
            0.0000001f * hostShare3.transform.localScale.z,

            0.0000001f * hostShare4.transform.localPosition.x,
            0.0000001f * hostShare4.transform.localPosition.y,
            0.0000001f * hostShare4.transform.localPosition.z,
            0.0000001f * hostShare4.transform.localScale.x,
            0.0000001f * hostShare4.transform.localScale.y,
            0.0000001f * hostShare4.transform.localScale.z,

            0.0000001f * hostShare5.transform.localPosition.x,
            0.0000001f * hostShare5.transform.localPosition.y,
            0.0000001f * hostShare5.transform.localPosition.z,
            0.0000001f * hostShare5.transform.localScale.x,
            0.0000001f * hostShare5.transform.localScale.y,
            0.0000001f * hostShare5.transform.localScale.z,

            0.0000001f * hostShare6.transform.localPosition.x,
            0.0000001f * hostShare6.transform.localPosition.y,
            0.0000001f * hostShare6.transform.localPosition.z,
            0.0000001f * hostShare6.transform.localScale.x,
            0.0000001f * hostShare6.transform.localScale.y,
            0.0000001f * hostShare6.transform.localScale.z,

            0.0000001f * hostShare7.transform.localPosition.x,
            0.0000001f * hostShare7.transform.localPosition.y,
            0.0000001f * hostShare7.transform.localPosition.z,
            0.0000001f * hostShare7.transform.localScale.x,
            0.0000001f * hostShare7.transform.localScale.y,
            0.0000001f * hostShare7.transform.localScale.z,

            0.0000001f * hostShare8.transform.localPosition.x,
            0.0000001f * hostShare8.transform.localPosition.y,
            0.0000001f * hostShare8.transform.localPosition.z,
            0.0000001f * hostShare8.transform.localScale.x,
            0.0000001f * hostShare8.transform.localScale.y,
            0.0000001f * hostShare8.transform.localScale.z,

            0.0000001f * hostShare9.transform.localPosition.x,
            0.0000001f * hostShare9.transform.localPosition.y,
            0.0000001f * hostShare9.transform.localPosition.z,
            0.0000001f * hostShare9.transform.localScale.x,
            0.0000001f * hostShare9.transform.localScale.y,
            0.0000001f * hostShare9.transform.localScale.z,

            0.0000001f * hostShare10.transform.localPosition.x,
            0.0000001f * hostShare10.transform.localPosition.y,
            0.0000001f * hostShare10.transform.localPosition.z,
            0.0000001f * hostShare10.transform.localScale.x,
            0.0000001f * hostShare10.transform.localScale.y,
            0.0000001f * hostShare10.transform.localScale.z,

            0.0000001f * hostShare11.transform.localPosition.x,
            0.0000001f * hostShare11.transform.localPosition.y,
            0.0000001f * hostShare11.transform.localPosition.z,
            0.0000001f * hostShare11.transform.localScale.x,
            0.0000001f * hostShare11.transform.localScale.y,
            //0.0000001f * hostShare11.transform.localScale.z,
        };
    }
}
