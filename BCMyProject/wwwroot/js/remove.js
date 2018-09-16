function Remove(photo) {
    let form = new FormData();
    form.append('photo', photo);

    fetch('/Home/RemoveFile', {
        method: "POST",
        credentials: 'include',
        body: form,
    })
        .then(res => {
            if (res.status === 200)
                RemoveImg(photo);
            else
                alert(res.status + ' - ' + res.statusText)
        })
        .catch(alert);
}

function RemoveImg(photo) {
    document.getElementById(photo).remove();
}