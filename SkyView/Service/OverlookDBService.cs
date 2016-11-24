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

        /// <summary>
        /// 景觀資料抓取
        /// </summary>
        /// <param name="scenic_id"></param>
        /// <param name="sort"></param>
        /// <param name="area_id"></param>
        /// <param name="status"></param>
        /// <param name="area_query"></param>
        /// <param name="scenic_query"></param>
        /// <returns></returns>
        public DataTable List(string scenic_id = "", string sort = "", string area_id = "", string status = "",string area_query = "",string scenic_query = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            string[] Array_scenic_id;
            string[] Array_area_id;
            string[] Array_area_query;
            string[] Array_scenic_query;
            string str_scenic_id = "";
            string str_area_id = "";
            string str_area_query = "";
            string str_scenic_query = "";

            if(area_id.Trim().Length > 0)
            {
                Array_area_id = area_id.Split(',');

                for (int i = 0; i < Array_area_id.Length; i++)
                {
                    if (i > 0)
                    {
                        str_area_id = str_area_id + ",";
                    }
                    str_area_id = str_area_id + "'" + Array_area_id[i] + "'";
                }
            }

            if (scenic_id.Trim().Length > 0)
            {
                Array_scenic_id = scenic_id.Split(',');

                for (int i = 0; i < Array_scenic_id.Length; i++)
                {
                    if (i > 0)
                    {
                        str_scenic_id = str_scenic_id + ",";
                    }
                    str_scenic_id = str_scenic_id + " " + Array_scenic_id[i] + " ";
                }
            }

            if (area_query.Trim().Length > 0)
            {
                Array_area_query = area_query.Split(',');
                str_area_query = "";
                str_area_query = str_area_query + "(";
                for (int i = 0; i < Array_area_query.Length; i++)
                {
                    if (i > 0)
                    {
                        str_area_query = str_area_query + " or ";
                    }
                    str_area_query = str_area_query + " a1.area_name like '%" + Array_area_query[i] + "%' ";
                }
                str_area_query = str_area_query + ")";
            }

            if (scenic_query.Trim().Length > 0)
            {
                Array_scenic_query = scenic_query.Split(',');
                str_scenic_query = "";
                str_scenic_query = str_scenic_query + "(";
                for (int i = 0; i < Array_scenic_query.Length; i++)
                {
                    if (i > 0)
                    {
                        str_scenic_query = str_scenic_query + " or ";
                    }
                    str_scenic_query = str_scenic_query + " a1.scenic_name like '%" + Array_scenic_query[i] + "%' ";
                }
                str_scenic_query = str_scenic_query + ")";
            }
            csql = "select "
                 + " a1.* "
                 + "from "
                 + "( "
                 //-----------------------------------------------------------//
                 + "select "
                 + "  a.*, b.area_name,c.img_file,d.img_id, e.img_file as img_file_b, e.img_desc as img_desc_b "
                 + "from "
                 + "  scenic_bt a "
                 + "left join area_bt b on a.area_id = b._SEQ_ID "
                 + "left join scenic_img c on a._SEQ_ID = c.img_no and img_sty = 'S' "
                 + "left join (select img_no,min(_SEQ_ID) as img_id from scenic_img where img_sty = 'B' group by img_no) d on a._SEQ_ID = d.img_no "
                 + "left join scenic_img e on d.img_id = e._SEQ_ID "
                 //-----------------------------------------------------------//
                 + ") a1 "
                 + "where "
                 + "  a1.status <> 'D' ";
            if(status.Trim().Length > 0)
            {
                csql = csql + " and a1.status = '" + status + "' ";
            }
            if(str_scenic_id.Trim().Length >0)
            {
                csql = csql + " and a1._SEQ_ID in (" + str_scenic_id + ") ";
            }

            if(str_area_id.Trim().Length >0)
            {
                csql = csql + " and a1.area_id in (" + str_area_id + ") ";
            }

            if(str_area_query.Trim().Length > 0)
            {
                csql = csql + " and " + str_area_query + " ";
            }

            if(str_scenic_query.Trim().Length > 0)
            {
                csql = csql + " and " + str_scenic_query + " ";
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
            string[] carea_id;
            string str_area_id = "";
            carea_id = area_id.Split(',');
            for(int i=0; i<carea_id.Length; i++)
            {
                if(i > 0)
                {
                    str_area_id = str_area_id + ",";
                }
                str_area_id = str_area_id + carea_id[i];
            }

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
                csql = csql + " and _SEQ_ID in (" + str_area_id + ") ";
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

        public string add_count(string scenic_id="")
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
                     + "  scenic_count = scenic_count + 1 "
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

        public DataTable Img_List(string img_no = "", string img_sty = "B")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);
            string[] cimg_no;
            string str_img_no = "";
            cimg_no = img_no.Split(',');

            for (int i = 0; i < cimg_no.Length ; i++)
            {
                if(i > 0)
                {
                    str_img_no = str_img_no + ",";
                }
                str_img_no = str_img_no + "'" + cimg_no[i] + "'";
            }

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = "select * from scenic_img where status = 'Y' and img_no in (" + str_img_no + ") and img_sty= '" + img_sty + "' order by _SEQ_ID ";


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

        public DataTable User_Info(string account = "")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);
            
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = "select "
                 + "  * "
                 + "from "
                 + "  member "
                 + "where "
                 + "   status <> 'D' "
                 + "and account = '" + account + "' "
                 + "order by "
                 + "  _SEQ_ID ";

            if (dt.Tables["user_info"] != null)
            {
                dt.Tables["user_info"].Clear();
            }

            SqlDataAdapter user_info_ada = new SqlDataAdapter(csql, conn);
            user_info_ada.Fill(dt, "user_info");
            user_info_ada = null;

            d_t = dt.Tables["user_info"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }

        public string User_Update(string account = "",string pwd = "")
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
                     + "  member "
                     + "set "
                     + "  pwd = '" + pwd + "' "
                     + "where "
                     + "  account = '" + account + "' ";

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
    }
}