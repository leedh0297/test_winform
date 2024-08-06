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
        private List<string[]> data = new List<string[]>(); // 데이터를 저장할 리스트
        private SortOrder sortOrder = SortOrder.Ascending; // 정렬 순서
        private int sortColumn = -1; // 정렬할 컬럼의 인덱스
        private int selectedIndex = -1; // 선택된 항목의 인덱스를 저장할 변수

        private string connectionString = "Host=192.168.201.151;Username=postgres;Password=12345678;Database=internTest"; // PostgreSQL 연결 문자열

        public Form1()
        {
            InitializeComponent();

            // ListView의 열을 동적으로 설정
            SetupListViewColumns();

            // 콤보박스 초기화
            SetupComboBox();

            // 데이터베이스에서 데이터 로드
            LoadDataFromDatabase();

            // 전체 도서 수 표시
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // ColumnClick 이벤트 핸들러 추가
            lv_list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);

            // SelectedIndexChanged 이벤트 핸들러 추가
            lv_list.SelectedIndexChanged += new EventHandler(lv_list_SelectedIndexChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // 테이블의 칼럼을 ListView에 동적으로 추가
        private void SetupListViewColumns()
        {
            lv_list.Columns.Clear();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            string dataType = reader["data_type"].ToString();

                            // 열 너비를 데이터 유형에 따라 결정 (여기서는 임의로 설정)
                            int columnWidth = dataType == "text" ? 300 : 100;

                            // 열 추가
                            lv_list.Columns.Add(columnName, columnWidth);
                        }
                    }
                }
            }

            lv_list.View = View.Details; // ListView를 상세보기로 설정
        }

        // 데이터베이스에서 데이터 로드
        private void LoadDataFromDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM book", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        data.Clear(); // 기존 데이터를 지움
                        while (reader.Read())
                        {
                            string[] row = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = reader[i].ToString();
                            }
                            data.Add(row); // 새 데이터를 추가
                        }
                    }
                }
            }
            UpdateListView(); // ListView를 업데이트
        }

        // 검색 버튼 클릭 시
        private void search_Click(object sender, EventArgs e)
        {
            string searchText = tb_search.Text.ToLower(); // 검색어를 소문자로 변환
            string selectedColumn = cb_filter.SelectedItem.ToString(); // 선택된 콤보박스 항목
            UpdateListView(selectedColumn, searchText); // ListView를 업데이트
        }

        // 삽입 버튼 클릭 시
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
                    // 파라미터 설정
                    cmd.Parameters.AddWithValue("title", title);
                    cmd.Parameters.AddWithValue("writer", writer);
                    cmd.Parameters.AddWithValue("genre", genre);
                    cmd.Parameters.AddWithValue("description", description);
                    cmd.ExecuteNonQuery(); // 쿼리 실행
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

        // 리스트 칼럼 클릭 시 정렬
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

        // ListViewItemComparer 클래스: ListView 정렬을 위한 클래스
        public class ListViewItemComparer : IComparer
        {
            private int col; // 정렬할 컬럼
            private SortOrder order; // 정렬 순서

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
                int returnVal;
                if (col == 0) // 첫 번째 열 (id 열)은 숫자형으로 정렬
                {
                    int id1 = int.Parse(((ListViewItem)x).SubItems[col].Text);
                    int id2 = int.Parse(((ListViewItem)y).SubItems[col].Text);
                    returnVal = id1.CompareTo(id2);
                }
                else
                {
                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                               ((ListViewItem)y).SubItems[col].Text);
                }

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
                ListViewItem selectedItem = lv_list.SelectedItems[0]; // 선택된 항목
                selectedIndex = lv_list.Items.IndexOf(selectedItem); // 선택된 항목의 인덱스 저장

                // 선택된 필터 컬럼을 가져옴
                string selectedColumn = cb_filter.SelectedItem.ToString();

                // '전체'가 선택된 경우 또는 체크박스가 체크되지 않은 경우
                if (selectedColumn == "전체" || !cb_column.Checked)
                {
                    // 모든 컬럼의 데이터를 텍스트박스에 출력
                    for (int i = 1; i < lv_list.Columns.Count; i++)
                    {
                        switch (i)
                        {
                            case 1:
                                tb_title.Text = selectedItem.SubItems[i].Text;
                                break;
                            case 2:
                                tb_writer.Text = selectedItem.SubItems[i].Text;
                                break;
                            case 3:
                                tb_genre.Text = selectedItem.SubItems[i].Text;
                                break;
                            case 4:
                                tb_description.Text = selectedItem.SubItems[i].Text;
                                break;
                        }
                    }
                }
                else
                {
                    // 체크박스가 체크되어 있고 '전체'가 아닌 다른 항목이 선택된 경우
                    for (int i = 0; i < lv_list.Columns.Count; i++)
                    {
                        // 현재 컬럼이 필터 컬럼과 일치할 때만 텍스트박스에 값 설정
                        if (lv_list.Columns[i].Text == selectedColumn)
                        {
                            switch (i)
                            {
                                case 1:
                                    tb_title.Text = selectedItem.SubItems[i].Text;
                                    break;
                                case 2:
                                    tb_writer.Text = selectedItem.SubItems[i].Text;
                                    break;
                                case 3:
                                    tb_genre.Text = selectedItem.SubItems[i].Text;
                                    break;
                                case 4:
                                    tb_description.Text = selectedItem.SubItems[i].Text;
                                    break;
                            }
                        }
                        else
                        {
                            // 필터 컬럼이 아닌 경우에는 텍스트박스를 비움
                            switch (i)
                            {
                                case 1:
                                    tb_title.Text = "";
                                    break;
                                case 2:
                                    tb_writer.Text = "";
                                    break;
                                case 3:
                                    tb_genre.Text = "";
                                    break;
                                case 4:
                                    tb_description.Text = "";
                                    break;
                            }
                        }
                    }
                }
            }
        }


        // 수정 버튼 클릭 시
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                int id = int.Parse(lv_list.SelectedItems[0].SubItems[0].Text); // 선택된 항목의 ID
                string title = tb_title.Text.Trim();
                string writer = tb_writer.Text.Trim();
                string genre = tb_genre.Text.Trim();
                string description = tb_description.Text.Trim();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("UPDATE book SET title = @title, writer = @writer, genre = @genre, description = @description WHERE id = @id", conn))
                    {
                        // 파라미터 설정
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.Parameters.AddWithValue("title", title);
                        cmd.Parameters.AddWithValue("writer", writer);
                        cmd.Parameters.AddWithValue("genre", genre);
                        cmd.Parameters.AddWithValue("description", description);
                        cmd.ExecuteNonQuery(); // 쿼리 실행
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

        // 삭제 버튼 클릭 시
        private void bt_delete_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                int id = int.Parse(lv_list.SelectedItems[0].SubItems[0].Text); // 선택된 항목의 ID

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("DELETE FROM book WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", id);
                        cmd.ExecuteNonQuery(); // 쿼리 실행
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

        // 새로고침 버튼 클릭 시
        private void bt_refresh_Click(object sender, EventArgs e)
        {
            // 데이터베이스에서 데이터 다시 로드
            LoadDataFromDatabase();

            // 전체 도서수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 콤보박스를 '전체'로 설정
            cb_filter.SelectedIndex = 0;

            // 검색어 텍스트박스 초기화
            tb_search.Text = "";

            // 텍스트박스 초기화
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";
        }

        // 콤보박스 초기화
        private void SetupComboBox()
        {
            // 콤보박스 초기화
            cb_filter.Items.Clear();
            cb_filter.Items.Add("전체");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            if (columnName != "id") // 'id' 열은 필터에 포함하지 않음
                            {
                                cb_filter.Items.Add(columnName);
                            }
                        }
                    }
                }
            }

            cb_filter.SelectedIndex = 0; // 기본 설정은 '전체'
        }

        // 콤보박스 선택 항목 변경 시
        private void cb_filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedColumn = cb_filter.SelectedItem.ToString(); // 선택된 콤보박스 항목
            string searchText = tb_search.Text.ToLower(); // 검색어를 소문자로 변환
            UpdateListView(selectedColumn, searchText); // ListView를 업데이트
        }

        // ListView 업데이트
        private void UpdateListView(string filterColumn = "", string searchText = "")
        {
            lv_list.Items.Clear();

            foreach (string[] row in data)
            {
                bool addRow = true;
                if (!string.IsNullOrEmpty(searchText))
                {
                    addRow = row.Any(col => col.ToLower().Contains(searchText));
                }

                if (addRow && !string.IsNullOrEmpty(filterColumn) && filterColumn != "전체")
                {
                    int columnIndex = lv_list.Columns.Cast<ColumnHeader>().ToList().FindIndex(c => c.Text == filterColumn);
                    if (columnIndex != -1)
                    {
                        addRow = row[columnIndex].ToLower().Contains(searchText);
                    }
                }

                if (addRow)
                {
                    ListViewItem item = new ListViewItem(row);
                    lv_list.Items.Add(item);
                }
            }
        }
    }
}