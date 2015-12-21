namespace StorageAdapters.Test
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

    public abstract partial class StorageAdapterTests<TFixture, TStorageAdapter> : IClassFixture<TFixture>
		where TStorageAdapter : IStorageAdapter
		where TFixture : StorageAdapterFixture<TStorageAdapter>, new()
	{
		[Fact]
		public async Task CreateDirectory_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.CreateDirectoryAsync(null);
			});
		}

		[Fact]
		public async Task CreateDirectory_1_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.CreateDirectoryAsync(null, true, System.Threading.CancellationToken.None);
			});
		}

		[Fact]
		public async Task DeleteDirectory_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.DeleteDirectoryAsync(null);
			});
		}

		[Fact]
		public async Task DeleteDirectory_1_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.DeleteDirectoryAsync(null, true, System.Threading.CancellationToken.None);
			});
		}

		[Fact]
		public async Task DeleteFile_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.DeleteFileAsync(null);
			});
		}

		[Fact]
		public async Task DirectoryExist_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.DirectoryExistAsync(null);
			});
		}

		[Fact]
		public async Task FileExists_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.FileExistAsync(null);
			});
		}

		[Fact]
		public async Task GetDirectories_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.GetDirectoriesAsync(null);
			});
		}

		[Fact]
		public async Task GetDirectoriesRecursive_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.GetDirectoriesRecursiveAsync(null);
			});
		}

		[Fact]
		public async Task GetFile_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.GetFileAsync(null);
			});
		}

		[Fact]
		public async Task GetFiles_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.GetFilesAsync(null);
			});
		}

		[Fact]
		public async Task GetFilesRecursive_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.GetFilesRecursiveAsync(null);
			});
		}

		[Fact]
		public async Task ReadFile_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.ReadFileAsync(null);
			});
		}

		[Fact]
		public async Task SaveFile_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				await StorageAdapter.SaveFileAsync(string.Empty, null);
			});
		}

		[Fact]
		public async Task SaveFile_1_ArgumentExceptions()
		{
			await Assert.ThrowsAsync(typeof(ArgumentNullException), async () =>
			{
				using (MemoryStream ms = new MemoryStream())
				{
					await StorageAdapter.SaveFileAsync(null, ms);
				}
			});
		}
	}
}
