using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SkyView.Service
{
    public class OverlookDBService
    {
        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string csql = "";
        DataSet ds = new DataSet();

        public DataTable List(string scenic_id = "", string sort = "", string area_id = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = "select "
                 + "  a.*, b.area_name,c.img_file "
                 + "from "
                 + "  scenic_bt a "
                 + "left join area_bt b on a.area_id = b._SEQ_ID "
                 + "left join scenic_img c on a._SEQ_ID = c.img_no and img_sty = 'S' "
                 + "where "
                 + "  a.status <> 'D' ";
            if(scenic_id.Trim().Length >0)
            {
                csql = csql + " and a._SEQ_ID = " + scenic_id + " ";
            }

            if(area_id.Trim().Length >0)
            {
                csql = csql + " and a.area_id = '" + area_id + "' ";
            }

            if(sort.Trim().Length > 0)
            {
                csql = csql + "order by " + sort + " ";
            }

            if(ds.Tables["scenic"] != null)
            {
                ds.Tables["scenic"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter(csql, conn);
            scenic_ada.Fill(ds, "scenic");
            scenic_ada = null;

            if(conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["scenic"];
        }

        //新增
        /// <summary>
        /// 景觀新增
        /// </summary>
        /// <param name="area_id">區域編號</param>
        /// <param name="scenic_name">景觀名稱</param>
        /// <param name="longitude">經度</param>
        /// <param name="latitude">緯度</param>
        /// <param name="scenic_desc">景觀描述</param>
        /// <param name="show">顯示狀態</param>
        /// <param name="img_no">圖片群編</param>
        /// <returns></returns>
        public string Insert(string area_id = "",string scenic_name = "", string longitude = "", string latitude = "",string scenic_desc = "",string show = "", string img_no = "")
        {
            string c_msg = "";
            string scenic_id = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "insert into scenic_bt(area_id,scenic_name,longitude,latitude,scenic_desc,status) "
                     + "values(" + area_id + ",'" + scenic_name + "','" + longitude + "','" + latitude + "','" + scenic_desc + "','" + show + "')";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();

                //抓取其編號
                csql = "select distinct "
                     + "  _SEQ_ID "
                     + "from "
                     + "   scenic_bt "
                     + "where "
                     + "    area_id = " + area_id + " "
                     + "and scenic_name = '" + scenic_name + "' "
                     + "and longitude = '" + longitude + "' "
                     + "and latitude = '" + latitude + "' "
                     + "and scenic_desc = '" + scenic_desc + "' "
                     + "and status = '" + show + "' ";
                if(ds.Tables["chk_scenic"] != null)
                {
                    ds.Tables["chk_scenic"].Clear();
                }

                SqlDataAdapter scenic_chk_ada = new SqlDataAdapter(csql, conn);
                scenic_chk_ada.Fill(ds, "chk_scenic");
                scenic_chk_ada = null;

                if(ds.Tables["chk_scenic"].Rows.Count > 0)
                {
                    scenic_id = ds.Tables["chk_scenic"].Rows[0]["_SEQ_ID"].ToString();
                    if (img_no.Trim().Length > 0)
                    {
                        csql = "update "
                             + "  scenic_img "
                             + "set "
                             + "  img_no = '" + scenic_id + "' "
                             + "where "
                             + "  img_no = '" + img_no + "' ";

                        cmd.CommandText = csql;
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch(Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }

        //更新內容
        public string Update(string scenic_id = "",string area_id = "", string scenic_name = "", string longitude = "", string latitude = "", string scenic_desc = "",string show = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "update "
                     + "  scenic_bt "
                     + "set "
                     + "  area_id = " + area_id + " "
                     + ", scenic_name = '" + scenic_name + "' "
                     + ", longitude = '" + longitude + "' "
                     + ", latitude = '" + latitude + "' "
                     + ", scenic_desc = '" + scenic_desc + "' "
                     + ", status = '" + show + "' "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  _SEQ_ID = " + scenic_id + " ";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }

        public string Del(string scenic_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "delete from "
                     + "  scenic_bt "
                     + "where "
                     + "  _SEQ_ID = " + scenic_id + " ";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        //更新狀態
        public string Upd_Status(string scenic_id = "", string status = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "update "
                     + "  scenic_bt "
                     + "set "
                     + "  status = '" + status + "' "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  _SEQ_ID = " + scenic_id + " ";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }

        //區域資料
        public DataTable AreaList(string area_id = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = "select "
                 + "  * "
                 + "from "
                 + "  area_bt "
                 + "where "
                 + "  status = 'Y' ";
            if(area_id.Trim().Length > 0)
            {
                csql = csql + " and _SEQ_ID = " + area_id + " ";
            }
            csql = csql + "order by _SEQ_ID ";
          

            if (ds.Tables["area"] != null)
            {
                ds.Tables["area"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter(csql, conn);
            scenic_ada.Fill(ds, "area");
            scenic_ada = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["area"];
        }

        public string Img_Insert(string img_no = "", string img_file = "", string img_sty = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "insert into scenic_img(img_no, img_file, img_sty) "
                     + "values(" + img_no + ",'" + img_file + "','" + img_sty + "')";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }

        public string Img_Delete(string img_id = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "delete from scenic_img where _SEQ_ID = " + img_id + " ";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }

        public string Img_Update(string img_id = "", string img_file = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "update "
                     + "  scenic_img "
                     + "set "
                     + "  img_file = '" + img_file + "' "
                     + "where "
                     + "  _SEQ_ID = " + img_id + " ";

                cmd.CommandText = csql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }

        public DataTable Img_List(string img_no = "",string img_sty = "B")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = "select * from scenic_img where status = 'Y' and img_no = '" + img_no + "' and img_sty= '" + img_sty + "' order by _SEQ_ID ";


            if (dt.Tables["img"] != null)
            {
                dt.Tables["img"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter(csql, conn);
            scenic_ada.Fill(dt, "img");
            scenic_ada = null;

            d_t = dt.Tables["img"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }
    }
}