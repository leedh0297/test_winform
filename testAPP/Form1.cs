using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace testAPP
{
    public partial class Form1 : Form
    {
        private List<string[]> data = new List<string[]>();
        private SortOrder sortOrder = SortOrder.Ascending;
        private int sortColumn = -1;
        private int selectedIndex = -1; // 선택된 항목의 인덱스를 저장할 변수

        private string connectionString = "Host=192.168.201.151;Username=postgres;Password=12345678;Database=internTest"; // PostgreSQL 연결 문자열

        public Form1()
        {
            InitializeComponent();

            // list뷰 칼럼 추가
            lv_list.Columns.Add("ID", 50);
            lv_list.Columns.Add("Title", 150);
            lv_list.Columns.Add("Writer", 100);
            lv_list.Columns.Add("Genre", 100);
            lv_list.Columns.Add("Description", 300);
            lv_list.View = View.Details;

            // 데이터베이스에서 데이터 로드
            LoadDataFromDatabase();

            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // ColumnClick 이벤트 핸들러 추가
            lv_list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);

            // SelectedIndexChanged 이벤트 핸들러 추가
            lv_list.SelectedIndexChanged += new EventHandler(lv_list_SelectedIndexChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // 데이터베이스에서 데이터 로드
        private void LoadDataFromDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id, title, writer, genre, description FROM book", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        data.Clear();
                        while (reader.Read())
                        {
                            string[] row = new string[]
                            {
                                reader["id"].ToString(),
                                reader["title"].ToString(),
                                reader["writer"].ToString(),
                                reader["genre"].ToString(),
                                reader["description"].ToString()
                            };
                            data.Add(row);
                        }
                    }
                }
            }
            UpdateListView();
        }

        // 검색
        private void search_Click(object sender, EventArgs e)
        {
            string searchText = tb_search.Text.ToLower();
            UpdateListView(searchText);
        }

        private void UpdateListView(string searchText = "")
        {
            lv_list.Items.Clear();

            foreach (string[] row in data)
            {
                if (string.IsNullOrEmpty(searchText) || row.Any(col => col.ToLower().Contains(searchText)))
                {
                    ListViewItem item = new ListViewItem(row);
                    lv_list.Items.Add(item);
                }
            }
        }

        // 삽입
        private void insert_Click(object sender, EventArgs e)
        {
            string title = tb_title.Text.Trim();
            string writer = tb_writer.Text.Trim();
            string genre = tb_genre.Text.Trim();
            string description = tb_description.Text.Trim();

            // 모든 텍스트박스가 빈칸이면 추가하지 않음
            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(writer) &&
                string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(description))
            {
                MessageBox.Show("모든 항목이 빈칸일 수 없습니다.");
                return;
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO book (title, writer, genre, description) VALUES (@title, @writer, @genre, @description)", conn))
                {
                    cmd.Parameters.AddWithValue("title", title);
                    cmd.Parameters.AddWithValue("writer", writer);
                    cmd.Parameters.AddWithValue("genre", genre);
                    cmd.Parameters.AddWithValue("description", description);
                    cmd.ExecuteNonQuery();
                }
            }

            // 데이터베이스에서 데이터 다시 로드
            LoadDataFromDatabase();

            // 전체 도서수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 텍스트박스 초기화
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";
        }


        // 리스트 칼럼 정렬
        private void list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
            {
                // 기존칼럼을 눌렀을때 정렬 방향을 변경
                sortOrder = (sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // 새로운 칼럼을 오름차순 정렬
                sortColumn = e.Column;
                sortOrder = SortOrder.Ascending;
            }

            // list뷰 정렬
            lv_list.ListViewItemSorter = new ListViewItemComparer(e.Column, sortOrder);
        }

        // ListViewItemComparer 클래스
        public class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;

            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                            ((ListViewItem)y).SubItems[col].Text);
                if (order == SortOrder.Descending)
                    returnVal *= -1;
                return returnVal;
            }
        }

        // 리스트뷰 항목 클릭 시 텍스트박스에 내용 출력
        private void lv_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_list.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lv_list.SelectedItems[0];
                selectedIndex = lv_list.Items.IndexOf(selectedItem); // 선택된 항목의 인덱스 저장
                tb_title.Text = selectedItem.SubItems[1].Text;
                tb_writer.Text = selectedItem.SubItems[2].Text;
                tb_genre.Text = selectedItem.SubItems[3].Text;
                tb_description.Text = selectedItem.SubItems[4].Text;
            }
        }

        // 수정
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                int id = int.Parse(lv_list.SelectedItems[0].SubItems[0].Text);
                string title = tb_title.Text.Trim();
                string writer = tb_writer.Text.Trim();
                string genre = tb_genre.Text.Trim();
                string description = tb_description.Text.Trim();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE book SET title = @title, writer = @writer, genre = @genre, description = @description WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Parameters.AddWithValue("title", title);
                        cmd.Parameters.AddWithValue("writer", writer);
                        cmd.Parameters.AddWithValue("genre", genre);
                        cmd.Parameters.AddWithValue("description", description);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 데이터베이스에서 데이터 다시 로드
                LoadDataFromDatabase();

                // 선택된 항목 초기화
                selectedIndex = -1;

                // 텍스트박스 초기화
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";
            }
            else
            {
                MessageBox.Show("수정할 항목을 선택하세요.");
            }
        }

        // 삭제
        private void bt_delete_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                int id = int.Parse(lv_list.SelectedItems[0].SubItems[0].Text);

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("DELETE FROM book WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 데이터베이스에서 데이터 다시 로드
                LoadDataFromDatabase();

                // 전체 도서수 업데이트
                total_books.Text = "전체 도서수 : " + data.Count.ToString();

                // 선택된 항목 초기화
                selectedIndex = -1;

                // 텍스트박스 초기화
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택하세요.");
            }
        }
    }
}
